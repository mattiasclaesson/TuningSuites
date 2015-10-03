#
# trimt8p
# Version: 0.1
#
import sys
import os.path
import base64
import binascii
import StringIO

# Debug-mode
debug = False

        
def main():

    if len(sys.argv) < 2:
        print("Usage: %s t8p_file_1 [t8p_file_N]" % sys.argv[0])
        exit(1)
   
    for idx, arg in enumerate(sys.argv):      
        if idx > 0:
            if not os.path.exists(arg):
                print "Input Tuning Pack File (.t8p) '%s' does not exist" % arg
                exit(1)
               
            # Open the input file
            print "Encoding input file : %s" % arg   
            fh = open(arg)
             
            # Read rest of file
            data = fh.readlines()

            # Close the input file
            fh.close()
            
            #fh = open(os.path.dirname(arg) + "\\" + os.path.basename(arg).split('.')[0] + "_trimmed.t8p", 'w')
            fh = open(arg, 'w')
            for line in data:
                if line != "":
                    fh.write(line.strip() + "\n")
                else:
                    fh.write(line)
            fh.close()
 

            # Define output file
            #out_file =  os.path.dirname(arg) + "\\" + os.path.basename(arg).split('.')[0] + ".t8x"

        
if __name__ == '__main__':
    main()