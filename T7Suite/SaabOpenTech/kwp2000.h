#ifndef __KWP2000_H__
#define __KWP2000_H__


#define MAX_NUMBER_OF_SESSIONS  10


enum responseCode { generalReject = 0x10,
                    serviceNotSupported = 0x11,
                    subFunctionNotSupported = 0x12,
                    busy_repeatRequest = 0x21,
                    conditionsNotCorrectOrRequestSeqError = 0x22,
                    routineNotCompleteorServiceInProgress = 0x23,
                    requestOutOfRange = 0x31,
                    securityAccessDenied = 0x33,
                    invalidKey = 0x35,
                    exceedNumberOfAttempts = 0x36,
                    requiredTimeDelayNotExpired = 0x37,
                    downloadNotAccepted = 0x40,
                    improperDownloadType = 0x41,
                    canNotDownloadToSpecifiedAddress = 0x42,
                    canNotDownloadNumberOfBytesRequested = 0x43,
                    uploadNotAccepted = 0x50,
                    improperUploadType = 0x51,
                    canNotUploadFromSpecifiedAddress = 0x52,
                    canNotUploadNumberOfBytesRequested = 0x53,
                    transferSuspended = 0x71,
                    transferAborted = 0x72,
                    illegalAddressInBlockTransfer = 0x74,
                    illegalByteCountInBlockTransfer = 0x75,
                    illegalBlockTransferType = 0x76,
                    blockTransferDataChecksumError = 0x77,
                    reqCorrectlyRcvd_RspPending = 0x78,
                    incorrectByteCountDuringBlockTransfer = 0x79,
                    serviceNotSupportedinActiveDiagSession = 0x80,
                    negativeResponse = 0x7F,
                    positiveResponse = 0x100,
                    responseTimeout = 0x101
                    };
                    
enum serviceIdCode {    startCommunication = 0x81,
                        stopCommunication = 0x82,
                        accessTimingParameters = 0x83,
                        testerPresent = 0x3E,
                        startDiagnosticSession = 0x10,
                        securityAccess = 0x27,
                        ecuReset = 0x11,
                        readEcuIdentification = 0x1A,
                        readDataByLocalIdentifier = 0x21,
                        readDataByCommonIdentifier = 0x22,
                        readMemoryByAddress = 0x23,
                        dynamicallyDefineLocalIdentifier = 0x2C,
                        writeDataByLocalIdentifier = 0x2C,
                        writeDataByCommonIdentifier = 0x2E,
                        writeMemoryByAddress = 0x3D,
                        readDiagnosticTroubleCodesByStatus = 0x18,
                        readStatusOfDiagnosticTroubleCodes = 0x17,
                        readFreezeFrameData = 0x12,
                        clearDiagnosticInformation = 0x14,
                        inputOutputControlByLocalIdentifier = 0x30,
                        inputOutputControlByCommonIdentifier = 0x2F,
                        startRoutineByLocalIdentifier = 0x31,
                        startRoutineByAddress = 0x38,
                        stopRoutineByLocalIdentifier = 0x32,
                        stopRoutineByAddress = 0x39,
                        requestRoutineResultsByLocalIdentifier = 0x33,
                        requestRoutineResultsByAddress = 0x3A,
                        requestDownload = 0x34,
                        requestUpload = 0x35,
                        transferData = 0x36,
                        requestTransferExit = 0x37
                    };

enum targetCode {   targetEHU = 0x91,
                    targetSID = 0x96,
                    targetACC = 0x98,
                    targetMIU = 0x9A,
                    targetTWICE = 0x9B,
                    targetTrionic = 0xA1,
                    targetNone = 0x00
                };

enum initCode {     initEHU = 0x81,
                    initSID = 0x65,
                    initACC = 0x98,
                    initMIU = 0x61,
                    initTWICE = 0x45,
                    initTrionic = 0x11,
                    initCDC = 0x28
                };

enum selectedDriver {   driverDebug,
                        driverCANUSB,
                        driverPCANUSB,
                        driverELM327CAN,
                        driverELM327OBD,
                        driverNone
                    };

struct messageKWP
{
    // "Data in"
    enum targetCode target;
    enum serviceIdCode serviceId;
    unsigned char lenParam;
    const unsigned char *param;
    // "Data out"
    unsigned char *lenRsp;
    unsigned char *response;
};


extern int startSessionKWP( enum initCode target );
extern enum responseCode sendRequestKWP( struct messageKWP *msg  );
extern int stopSessionKWP( enum initCode target );
extern int initKWP( enum selectedDriver driver );
extern int keepAliveSessionsKWP( void );

#endif // __KWP2000_H__
