#
# t8p Encrypt
# Version: 0.1
#
import sys
import os.path
import base64
import binascii
import StringIO
from Crypto.Cipher import AES


# Debug-mode
debug = False

class PKCS7Encoder(object):
    def __init__(self, k=16):
        self.k = k
    
    def decode(self, text):
        nl = len(text)
        val = int(binascii.hexlify(text[-1]), 16)
        if val > self.k:
            raise ValueError('Input is not padded or padding is corrupt')
        l = nl - val
        return text[:l]
        
    def encode(self, text):
        l = len(text)
        output = StringIO.StringIO()
        val = self.k - (l % self.k)
        for _ in xrange(val):
            output.write('%02x' % val)
        return text + binascii.unhexlify(output.getvalue())

def encrypt_AES(message):
    key = 'T8SuiteTunesSaab'
    iv = '1234567812345678'
    aes = AES.new(key, AES.MODE_CBC, iv)
    encoder = PKCS7Encoder()
    pad_text = encoder.encode(message)    
    cipher = aes.encrypt(pad_text)
    enc_cipher = base64.b64encode(cipher)
    return enc_cipher
    
def decrypt_AES(package):
    key = 'T8SuiteTunesSaab'
    iv = '1234567812345678'
    aes = AES.new(key, AES.MODE_CBC, iv)
    decoder = PKCS7Encoder()   
    dec = base64.b64decode(package)
    decipher = aes.decrypt(dec)
    unpad_text = decoder.decode(decipher)
    return unpad_text
    
def encode_file(encoded_file, in_data):

    # Heading
    enc_file = open(encoded_file, 'w')    
    
    # Encode
    for data in in_data:
        enc_file.write(encrypt_AES(data)+ "\n")
    
    # Close
    enc_file.close()

def decode_file(encoded_file):

    # Heading
    enc_file = open(encoded_file, 'r')    
    in_data = enc_file.readlines()

    # Decode
    out = ""
    for data in in_data:
        out = out + decrypt_AES(data)
    
    # Close
    enc_file.close()
    return out
   
def main():

    if len(sys.argv) < 2:
        print("Usage: %s t8p_file_1 [t8p_file_N]" % sys.argv[0])
        exit(1)
   
    for idx, arg in enumerate(sys.argv):      
        if idx > 0: #t8p file
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

            # Define output file
            out_file = arg.split('.')[0] + ".t8x"
 
            # Create output files
            encode_file(out_file, data)
            
            if debug:
                test_file = open(str(idx) + 'test.t8p', 'w')
                test_file.write(decode_file(out_file))
                test_file.close()
        
if __name__ == '__main__':
    main()