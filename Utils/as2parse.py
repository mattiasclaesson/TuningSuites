#
# AS2 Parser
# Version: 0.4
#
import sys, getopt

# Debug-mode
debug = False

#
SYMBOL_NAME = 				'dict_symbol_name'
SYMBOL_UNIT_OF_MEASURE = 	'dict_symbol_unit_of_measure'
SYMBOL_UNIT_VAL = 			'dict_ssymbol_unit_val'
SYMBOL_DETAILS = 			'dict_symbol_details'
SYBOL_DESCRIPTION = 		'dict_symbol_description'

# Counters
num_SCALAR = 0
num_MAP = 0
num_TABLE = 0
num_TABLENOSP = 0

symbol_list = []

#my_symbol[SYMBOL_NAME]
#my_symbol[SYMBOL_DETAILS]
#my_symbol[SYMBOL_UNIT_OF_MEASURE]
#my_symbol[SYMBOL_UNIT_VAL]
#my_symbol[SYBOL_DESCRIPTION]

def create_files(out_file_form1, out_sym_trans):
    in_code  = """using System;
using System.Collections.Generic;
using System.Text;
using T8SuitePro;

namespace T8SuitePro
{
    class SymbolTranslator
    {
        public string TranslateSymbolToHelpText(string symbolname, out string helptext, out XDFCategories category, out XDFSubCategory subcategory)
        {
            if (symbolname.EndsWith("!")) symbolname = symbolname.Substring(0, symbolname.Length - 1);
            helptext = "";
            category = XDFCategories.Undocumented;
            subcategory = XDFSubCategory.Undocumented;
            string description = "";
            switch (symbolname)
            {    
"""
    out_code = """            }
            return description;
        }
    }
}
    """
    switch_code = """                case "THE_SYMBOL_NAME":
                    description = helptext = "THE_SYMBOL_DESCRIPTION";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
"""
    if debug:
        print symbol_list
    
    fh_form1 = open(out_file_form1, 'w')    
    fh_form1.write('            /** vanaf hier */\n\n')
    fh_sym = open(out_sym_trans, 'w')    
    fh_sym.write(in_code)
    for a_symbol in symbol_list:
        fh_form1.write('            else if (symbolname == \"' + a_symbol[SYMBOL_NAME] + '\") returnvalue = ' + str(a_symbol[SYMBOL_UNIT_VAL]) +';\n')
        my_desc = ''
        if a_symbol.has_key(SYBOL_DESCRIPTION):
            my_desc = a_symbol[SYBOL_DESCRIPTION]
        fh_sym.write(switch_code.replace('THE_SYMBOL_NAME', a_symbol[SYMBOL_NAME]).replace('THE_SYMBOL_DESCRIPTION', my_desc))

    fh_form1.write('\n\n            /** tot hier **/\n')
    fh_sym.write(out_code)
    fh_form1.close()
    fh_sym.close()
        


def parse_symbol_data(symbol_name, symbol_data):
    global num_SCALAR
    global num_MAP
    global num_TABLE
    global num_TABLENOSP
    
    my_symbol = {};
    my_symbol[SYMBOL_NAME] = symbol_name
    for index, item in enumerate(symbol_data):
        if index == 0:
            if item.strip() == "SCALAR":
                num_SCALAR = num_SCALAR + 1;
            elif item.strip() == "MAP":
                num_MAP = num_MAP + 1;
            elif item.strip() == "TABLE":
                num_TABLE = num_TABLE + 1;
            elif item.strip() == "TABLENOSP":
                num_TABLENOSP = num_TABLENOSP + 1;
                
        elif index == 1:
            my_symbol[SYMBOL_DETAILS] = item.split()
            if debug:
                print(my_symbol[SYMBOL_DETAILS])
            
            # Save symbol unit type, if exist
            my_symbol[SYMBOL_UNIT_OF_MEASURE] = "N/A"
            if len(my_symbol[SYMBOL_DETAILS]) == 9:
                my_symbol[SYMBOL_UNIT_OF_MEASURE] = my_symbol[SYMBOL_DETAILS][8]
                if my_symbol[SYMBOL_UNIT_OF_MEASURE] == '\xb0':
                    my_symbol[SYMBOL_UNIT_OF_MEASURE] = 'degrees'
            if debug:
                print(my_symbol[SYMBOL_UNIT_OF_MEASURE])
                
            # Save symbol unit value
            my_symbol[SYMBOL_UNIT_VAL] = float(my_symbol[SYMBOL_DETAILS][0]) / float(my_symbol[SYMBOL_DETAILS][1])
            if debug:
                print(my_symbol[SYMBOL_UNIT_VAL])
        else:
            if item.startswith('Desc'):
                my_symbol[SYBOL_DESCRIPTION] = item.split('"')[1].strip().replace('\xb0', 'degrees')
                if debug:
                    print "STA: " + my_symbol[SYBOL_DESCRIPTION]
            else:
                if item[0] == ' ':
                    if len(item.split('"')) == 2:
                        my_symbol[SYBOL_DESCRIPTION] = my_symbol[SYBOL_DESCRIPTION] + ' ' + item.split('"')[0].strip().replace('\xb0', 'degrees')
                        if debug:
                            print "END: " + item.split('"')[0].strip().replace('\xb0', 'degrees')
                    else:
                        my_symbol[SYBOL_DESCRIPTION] = my_symbol[SYBOL_DESCRIPTION] + ' ' + item.strip().replace('\xb0', 'degrees')
                        if debug:
                            print "MID: " + item.strip()
    if debug:
        print my_symbol
        
    return my_symbol
        
def main():
    global num_SCALAR
    global num_MAP
    global num_TABLE
    global num_TABLENOSP
    global symbol_list
    ifile=''
    ffile='Form1_contrib.cs'
    sfile='SymbolTranslator.cs'
 
    # Read command line args
    try:
        myopts, args = getopt.getopt(sys.argv[1:],"i:f:s:")
    except getopt.GetoptError as e:
        print (str(e))
        print("Usage: %s -i input [-f form1_filename] [-s symbol_translator_filename]" % sys.argv[0])
        sys.exit(2)     

    for o, a in myopts:
        if o == '-i':
            ifile=a
        elif o == '-f':
            ffile=a
        elif o == '-s':
            sfile=a
        else:
            print("Usage: %s -i input [-f form1_filename] [-s symbol_translator_filename]" % sys.argv[0])
        
    if ifile == '':
        print "No input file provided."
        print("Usage: %s -i input [-f form1_filename] [-s symbol_translator_filename]" % sys.argv[0])
        sys.exit(2)
        
    print ("Input file : %s\nForm1 file: %s\nSymbolTranslator file: %s" % (ifile,ffile,sfile) )    
    fh = open(ifile)
    
    # Read headers
    file_version = fh.readline().strip()
    symbols = int(fh.readline().split(':')[1].strip())

    if debug:
        print("File version:      " + file_version)
        print("Number of symbols: " + str(symbols))
    
    data = fh.readlines()
    searching = True
    symbol_data = []
    for line in data:
        if searching:
            if line[0] == '*':
                symbol_name = line[1:].strip()
                if debug:
                    print("Found symbol: " + symbol_name)
                searching = False
        else:
            if line[0] == '\n':
                if debug:
                    print("Found end of symbol " + symbol_name)
                symbol_list.append(parse_symbol_data(symbol_name, symbol_data))
                searching = True
                symbol_data = []
            else:
                symbol_data.append(line)
 
    # Create output files
    create_files(ffile,sfile)
    print "Parsed %s symbols successfully" % str(num_SCALAR + num_MAP + num_TABLE + num_TABLENOSP)

    # Check file consistency
    if symbols != num_SCALAR + num_MAP + num_TABLE + num_TABLENOSP:
        print("Mismatch!")
        print("Number of symbols to be found:" + str(symbols))
        print("Number of symbols found: " + str(num_SCALAR + num_MAP + num_TABLE + num_TABLENOSP))
        print(" - SCALAR:  " + str(num_SCALAR))
        print(" - MAP:     " + str(num_MAP))
        print(" - TABLE:   " + str(num_TABLE))
        print(" - TABLESP: " + str(num_TABLENOSP))
    if debug:
        print symbol_list
        
if __name__ == '__main__':
    main()