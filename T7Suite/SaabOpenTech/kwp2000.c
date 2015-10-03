#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include "kwp2000.h"

#define FALSE   0
#define TRUE    1

#define KEEP_ALIVE_TIME     3000        // milliseconds

/* Structs */
struct sessionData
{
    unsigned char sessionOpen;
    enum initCode target;
    DWORD timeOfLastRequest;
};

/* Static variables */
struct sessionData activeSessions[MAX_NUMBER_OF_SESSIONS];
enum selectedDriver driverKWP;

/* Function prototypes */
int sendDebugOutput( const unsigned char *data );
int receiveDebugInput( unsigned char *data );



int initKWP( enum selectedDriver driver )
{
    unsigned char i;
    
    for( i = 0; i < MAX_NUMBER_OF_SESSIONS; i++ )
    {
        activeSessions[i].sessionOpen = FALSE;
        activeSessions[i].target = targetNone;
        activeSessions[i].timeOfLastRequest = 0;
    }
    
    driverKWP = driver;
    
    return 0;
}

int startSessionKWP( enum initCode target )
{
    unsigned char i;

    for( i = 0; i < MAX_NUMBER_OF_SESSIONS; i++ )
    {
        if( activeSessions[i].sessionOpen == FALSE )
        {
            activeSessions[i].sessionOpen = TRUE;
            activeSessions[i].target = target;
            activeSessions[i].timeOfLastRequest = GetTickCount();
            break;
        }
    }
    
    if( i == MAX_NUMBER_OF_SESSIONS )
    {
        // No available slots found...
        return -1;
    }
    
    switch( driverKWP )
    {
        case driverDebug:
        case driverNone:
            // do nothing
            break;

        case driverCANUSB:
            
            break;

        default:
            // invalid driver
            return -1;
            break;
    }
    
    return 0;
}



int stopSessionKWP( enum initCode target )
{
    unsigned char i;
    
    for( i = 0; i < MAX_NUMBER_OF_SESSIONS; i++ )
    {
        if( activeSessions[i].target == target )
        {
            activeSessions[i].sessionOpen = FALSE;
            activeSessions[i].target = targetNone;
            activeSessions[i].timeOfLastRequest = 0;
            break;
        }
    }
    
    if( i == MAX_NUMBER_OF_SESSIONS )
    {
        // Session not found...
        return -1;
    }
    
    return 0;    
}

int keepAliveSessionsKWP( void )
{
    unsigned char i;
    struct messageKWP msg;
    
    for( i = 0; i < MAX_NUMBER_OF_SESSIONS; i++ )
    {
        if( activeSessions[i].sessionOpen == TRUE && 
            activeSessions[i].target != targetNone )
        {
            if( activeSessions[i].timeOfLastRequest - GetTickCount() > KEEP_ALIVE_TIME )
            {
                msg.target = activeSessions[i].target;
                msg.serviceId = testerPresent;
                msg.lenParam = 0;
                msg.param = NULL;
                msg.lenRsp = NULL;
                msg.response = NULL;
                
                sendRequestKWP( &msg );
            }
        }
    }
    
}


enum responseCode sendRequestKWP( struct messageKWP *msg  )
{
    unsigned char data[8];
    unsigned char rowCount, byteCount, i, k, rcvdLen, rcvdCount, rcvdRowCount;
    unsigned char firstRowRcvd;
    int row;
    const unsigned char *ptrParam;


    // Parse together the message to be sent
    rowCount = (unsigned char)floor( (float)(msg->lenParam + 1) / 6.0 );
    ptrParam = msg->param;
    byteCount = 0;

    for( row = rowCount; row >= 0; row-- )
    {
        // Handle first row differently
        if( row == rowCount )
        {
            data[0] = row | 0x40;
            data[1] = msg->target;
            data[2] = msg->lenParam + 1;    // add serviceId to length
            data[3] = msg->serviceId;
            for( i = 0; i < 4; i++ )
            {
                if( msg->lenParam > i )
                {
                    data[4+i] = *ptrParam++;
                    byteCount++;
                }
                else
                    data[4+i] = 0x00;
            }
        }
        else
        {
            data[0] = row;
            data[1] = msg->target;
            for( i = 2; i < 8; i++ )
            {
                if( msg->lenParam > byteCount )
                {
                    data[i] = *ptrParam++;
                    byteCount++;
                }
                else
                    data[i] = 0x00;
            }
        }

        // Send data
        sendDebugOutput( data );
    }

    // Receive first (and possibly only) response row
    rcvdCount = 0;
    firstRowRcvd = 0;
    while( firstRowRcvd == 0 )
    {
        if( receiveDebugInput( data ) == 0 )
        {
            rcvdRowCount = data[0] & ~0xC0;
            for( i = 0; i < 8; i++ ) printf("0x%02X ", data[i] );
            printf("\n");
            if( (enum responseCode)data[3] == negativeResponse )
            {
                if( (enum serviceIdCode)data[4] == msg->serviceId &&
                    (enum responseCode)data[5]  == reqCorrectlyRcvd_RspPending )
                {
                    // Server busy, wait for another response, do not send
                    // anything (not even testerPresent requests) to server!
                    printf("server busy!\n");
                }
                else
                {
                    printf("negative response 0x%02X !\n", data[5] );
                }
            }
            else if( (enum serviceIdCode)(data[3] & ~0x40) == msg->serviceId )
            {
                rcvdLen = data[2] - 1;          // remove serviceId from length
                for(i = 0; i < 4; i++ )
                {
                    if( rcvdLen - rcvdCount > 0 )
                    {
                        if( msg->response != NULL )
                        {
                            *msg->response = data[4+i];
                            msg->response++;
                        }
                        rcvdCount++;
                    }
                }
                // Ok, ready to move on to the next responses (if any)
                firstRowRcvd = 1;
            }
            else
            {
                // response malformed
                printf("code=0x%02X, 0x%02X\n", data[3] & ~0x40, msg->serviceId );
                return generalReject;
            }
        }
        else
        {
            return responseTimeout;
        }
    }

    // Receive rest of the response rows (if any)
    while( rcvdRowCount > 0x00 )
    {
        if( receiveDebugInput( data ) == 0 )
        {
            // just to be safe, mask the two upper bits (they shouldn't occur)
            rcvdRowCount = data[0] & ~0xC0;

            for(i = 0; i < 6; i++ )
            {
                if( rcvdLen - *msg->lenRsp > 0 )
                {
                    if( msg->response != NULL )
                    {
                        *msg->response = data[2+i];
                        msg->response++;
                    }
                    rcvdCount++;
                }
            }
            
        }
        else
        {
            if( msg->lenRsp != NULL ) *msg->lenRsp = rcvdCount;
            return responseTimeout;
        }
    }
    

    if( msg->lenRsp != NULL ) *msg->lenRsp = rcvdCount;
    return positiveResponse;
}


int sendMessage( const unsigned char *data )
{
    
    switch( driverKWP )
    {
        case driverNone:
            return 0;
        
        case driverDebug:
            return sendDebugOutput( data );

        case driverCANUSB:
            //return sendCANUSB( 0x240, data );
            return 0;
            
        default:
            return -1;
    }
}

int sendDebugOutput( const unsigned char *data )
{
    unsigned char k;

    for( k = 0; k < 8; k++ ) printf("0x%02X ", *(data+k) );
    printf("\n");

    return 0;    
}

unsigned char debuginput1[] = { 0xC0, 0xBF, 0x03, 0x7F, 0x30, 0x78, 0x00, 0x00 };
unsigned char debuginput2[] = { 0xC0, 0xBF, 0x03, 0x70, 0x3E, 0x06, 0x00, 0x00 };

int receiveDebugInput( unsigned char *data )
{
    unsigned char i;
    static debugCounter = 1;
    
    if( debugCounter )
    {
        for( i = 0; i < 8; i++ ) *(data+i) = debuginput1[i];
        debugCounter = 0;
        printf("#");
    }
    else
    {
        for( i = 0; i < 8; i++ ) *(data+i) = debuginput2[i];
        debugCounter = 1;
        printf("$");
    }    
    
    return 0;
}
