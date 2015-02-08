#
# t8p Encrypt
# Version: 0.2
#
import sys
import os.path
import base64
import binascii
import StringIO
from Crypto.Cipher import AES
from Crypto.PublicKey import RSA
from Crypto.Cipher import PKCS1_OAEP
from Crypto.Signature import PKCS1_PSS
from Crypto.Signature import PKCS1_v1_5
from Crypto.Hash import SHA
import hashlib

# Debug-mode
debug = False

def sign_RSA(message):
    '''
    param: public_key_loc Path to public key
    param: message String to be encrypted
    return base64 encoded encrypted string
    '''
    key = open('T8Priv.pem', "r").read()
    h = SHA.new(message)
    rsakey = RSA.importKey(key)
    signer = PKCS1_v1_5.new(rsakey)
    signature = signer.sign(h)
    return signature.encode('base64') 

def check_sign_RSA(message, signature):
    key = open('T8Priv.pem', "r").read()
    rsakey = RSA.importKey(key)
    h = SHA.new(message)
    verifier = PKCS1_v1_5.new(rsakey)
    if verifier.verify(h, signature.decode('base64')):
        return True
    else:
        return False


def encrypt_RSA(message):
    '''
    param: public_key_loc Path to public key
    param: message String to be encrypted
    return base64 encoded encrypted string
    '''
    key = open('T8Pub.pem', "r").read()
    rsakey = RSA.importKey(key)
    rsakey = PKCS1_OAEP.new(rsakey)
    encrypted = rsakey.encrypt(message)
    return encrypted.encode('base64') 
       
def decrypt_RSA(package):
    '''
    param: public_key_loc Path to your private key
    param: package String to be decrypted
    return decrypted string
    '''
    from Crypto.PublicKey import RSA
    from Crypto.Cipher import PKCS1_OAEP
    from base64 import b64decode
    key = open('T8Pub.pem', "r").read()
    rsakey = RSA.importKey(key)
    rsakey = PKCS1_OAEP.new(rsakey)
    decrypted = rsakey.decrypt(b64decode(package))
    return decrypted 
    
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
    
def file_prepender(filename, data):
    with open(filename, 'r+') as f:
        content = f.read()
        f.seek(0, 0)
        f.write("<SIGNATURE>\n" + data + "</SIGNATURE>\n"+ content)   
        
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
            #out_file = os.path.basename(arg).split('.')[0] + ".t8x"
            out_file =  os.path.dirname(arg) + "\\" + os.path.basename(arg).split('.')[0] + ".t8x"
 
            # Create output files
            encode_file(out_file, data)
            
            # Create a MD5 hash of original file
            hash = hashlib.md5(open(arg, 'rb').read()).hexdigest()
            if debug:
                print hash
                
            # Create a signature out of MD5 hash
            signature = sign_RSA(hash)
            if debug:
                if(check_sign_RSA(hash, signature)):
                    print "Signature OK"
            
            # Add signature at beginning of file
            file_prepender(out_file, signature)
        
if __name__ == '__main__':
    main()