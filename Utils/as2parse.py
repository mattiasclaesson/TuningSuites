#
# AS2 Parser
# Version: 0.8
#
import sys, getopt

# Debug-mode
debug = False

#
SYMBOL_NAME = 				'dict_symbol_name'
SYMBOL_TYPE = 				'dict_symbol_type'
SYMBOL_UNIT_OF_MEASURE = 	'dict_symbol_unit_of_measure'
SYMBOL_UNIT_VAL = 			'dict_ssymbol_unit_val'
SYMBOL_DETAILS = 			'dict_symbol_details'
SYBOL_DESCRIPTION = 		'dict_symbol_description'
SYMBOL_X_AXIS =             'dict_x_axis'
SYMBOL_X_AXIS_FUNC =        'dict_x_axis_func'
SYMBOL_Y_AXIS =             'dict_y_axis'
SYMBOL_Y_AXIS_FUNC =        'dict_y_axis_func'

TYPE_SCALAR = 'SCALAR'
TYPE_MAP = 'MAP'
TYPE_TABLE = 'TABLE'
TYPE_TABLENOSP = 'TABLENOSP'

# Counters
num_SCALAR = 0
num_MAP = 0
num_TABLE = 0
num_TABLENOSP = 0
symbol_list = []

def create_files(out_sym_dict_file):
    in_sym_dict = """using System;
using System.Collections.Generic;
namespace T8SuitePro
{
    class MySymbol
    {
        public string type;
        public double unitVal;
        public string unitOfMeasure;
        public string description;
        public string xAxis;
        public string xAxisFunction;
        public string yAxis;
        public string yAxisFunction;
        
        public MySymbol(string inType, double inUnitVal, string inUnitOfMeasure, 
                        string inDescription, string inXAxis, string inXAxisFunction, 
                        string inYAxis, string inYAxisFunction)
        {
            type = inType;
            unitVal = inUnitVal;
            unitOfMeasure = inUnitOfMeasure;
            description = inDescription;
            xAxis = inXAxis;
            xAxisFunction = inXAxisFunction;
            yAxis = inYAxis;
            yAxisFunction = inYAxisFunction;
        }
    }
    static class SymbolDictionary
    {
        static Dictionary<string, MySymbol> _dict = new Dictionary<string, MySymbol>
        {"""
            
            
    out_sym_dict = """
            {"END", new MySymbol("",0,"","", "","","","")}
        };

        public static double GetSymbolUnit(string word)
        {
            // Try to get the result in the static Dictionary
            MySymbol result;
            if (_dict.TryGetValue(word, out result))
            {
                return result.unitVal;
            }
            else
            {
                return 0;
            }
        }
        public static string GetSymbolUnitOfMeasure(string word)
        {
            // Try to get the result in the static Dictionary
            MySymbol result;
            if (_dict.TryGetValue(word, out result))
            {
                return result.unitOfMeasure;
            }
            else
            {
                return "";
            }
        }
        public static string GetSymbolDescription(string word)
        {
            // Try to get the result in the static Dictionary
            MySymbol result;
            if (_dict.TryGetValue(word, out result))
            {
                return result.description;
            }
            else
            {
                return "";
            }
        }
        public static string GetSymbolXAxis(string word)
        {
            // Try to get the result in the static Dictionary
            MySymbol result;
            if (_dict.TryGetValue(word, out result))
            {
                return result.xAxis;
            }
            else
            {
                return "";
            }
        }
        public static string GetSymbolYAxis(string word)
        {
            // Try to get the result in the static Dictionary
            MySymbol result;
            if (_dict.TryGetValue(word, out result))
            {
                return result.yAxis;
            }
            else
            {
                return "";
            }
        }
        public static string GetSymbolXAxisFunction(string word)
        {
            // Try to get the result in the static Dictionary
            MySymbol result;
            if (_dict.TryGetValue(word, out result))
            {
                return result.xAxisFunction;
            }
            else
            {
                return "";
            }
        }
        public static string GetSymbolYAxisFunction(string word)
        {
            // Try to get the result in the static Dictionary
            MySymbol result;
            if (_dict.TryGetValue(word, out result))
            {
                return result.yAxisFunction;
            }
            else
            {
                return "";
            }
        }
    }
}"""

    # Heading
    fh_sym_dic = open(out_sym_dict_file, 'w')    
    fh_sym_dic.write(in_sym_dict)
    
    # Repetetive
    for a_symbol in symbol_list:
        fh_sym_dic.write('\n            {"' + 
                            a_symbol[SYMBOL_NAME] + '\", new MySymbol(\"' +\
                            a_symbol[SYMBOL_TYPE] +               '\", ' +\
                            str(a_symbol[SYMBOL_UNIT_VAL]) +           ', \"' +\
                            a_symbol[SYMBOL_UNIT_OF_MEASURE] +    '\", \"' +\
                            a_symbol[SYBOL_DESCRIPTION] +         '\", \"' +\
                            a_symbol[SYMBOL_X_AXIS] +             '\", \"' +\
                            a_symbol[SYMBOL_X_AXIS_FUNC] +        '\", \"' +\
                            a_symbol[SYMBOL_Y_AXIS] +             '\", \"' +\
                            a_symbol[SYMBOL_Y_AXIS_FUNC] +        '\")},')
    # Tail
    fh_sym_dic.write(out_sym_dict)
    fh_sym_dic.close()
        
def parse_symbol_data(symbol_name, symbol_data):
    global num_SCALAR
    global num_MAP
    global num_TABLE
    global num_TABLENOSP
    
    my_symbol = {};
    my_symbol[SYMBOL_NAME] = symbol_name
    my_symbol[SYMBOL_X_AXIS] = ""
    my_symbol[SYMBOL_X_AXIS_FUNC] = ""
    my_symbol[SYMBOL_Y_AXIS] = ""
    my_symbol[SYMBOL_Y_AXIS_FUNC] = ""
    my_symbol[SYBOL_DESCRIPTION] = ""
    desc_index = 0
    
    # Store symbol type
    if symbol_data[0].strip() == TYPE_SCALAR:
        num_SCALAR = num_SCALAR + 1
        my_symbol[SYMBOL_TYPE] = TYPE_SCALAR
        desc_index = 2
        
    elif symbol_data[0].strip() == TYPE_MAP:
        num_MAP = num_MAP + 1
        my_symbol[SYMBOL_TYPE] = TYPE_MAP
        desc_index = 6
        my_symbol[SYMBOL_X_AXIS] = symbol_data[2].strip()
        my_symbol[SYMBOL_X_AXIS_FUNC] = "Function of: " + symbol_data[3].strip()
        my_symbol[SYMBOL_Y_AXIS] = symbol_data[4].strip()
        my_symbol[SYMBOL_Y_AXIS_FUNC] = "Function of: " + symbol_data[5].strip()

    elif symbol_data[0].strip() == TYPE_TABLE:
        num_TABLE = num_TABLE + 1
        my_symbol[SYMBOL_TYPE] = TYPE_TABLE
        desc_index = 4
        my_symbol[SYMBOL_Y_AXIS] = symbol_data[2].strip()
        my_symbol[SYMBOL_Y_AXIS_FUNC] = "Function of: " + symbol_data[3].strip()

    elif symbol_data[0].strip() == TYPE_TABLENOSP:
        num_TABLENOSP = num_TABLENOSP + 1
        my_symbol[SYMBOL_TYPE] = TYPE_TABLENOSP
        desc_index = 2

    # Store and extract symbol details
    my_symbol[SYMBOL_DETAILS] = symbol_data[1].split()
    
    # Save symbol unit type, if exist
    my_symbol[SYMBOL_UNIT_OF_MEASURE] = "N/A"
    if len(my_symbol[SYMBOL_DETAILS]) == 9:
        my_symbol[SYMBOL_UNIT_OF_MEASURE] = my_symbol[SYMBOL_DETAILS][8]
        
    # Save symbol unit value
    my_symbol[SYMBOL_UNIT_VAL] = float(my_symbol[SYMBOL_DETAILS][0]) / float(my_symbol[SYMBOL_DETAILS][1])
    
    for i in range(desc_index, len(symbol_data)):
        if symbol_data[i].startswith('Desc'):
            my_symbol[SYBOL_DESCRIPTION] = symbol_data[i].split('"')[1].strip()
        else:
            if symbol_data[i][0] == ' ':
                if len(symbol_data[i].split('"')) == 2:
                    my_symbol[SYBOL_DESCRIPTION] = my_symbol[SYBOL_DESCRIPTION] + ' ' + symbol_data[i].split('"')[0].strip().replace('"', '\'')
                else:
                    my_symbol[SYBOL_DESCRIPTION] = my_symbol[SYBOL_DESCRIPTION] + ' ' + symbol_data[i].strip().replace('"', '\'')
                        
    return my_symbol
   
def main():
    global num_SCALAR
    global num_MAP
    global num_TABLE
    global num_TABLENOSP
    global symbol_list
    ifile=''
    dfile='SymbolDictionary.cs'
 
    # Read command line args
    try:
        myopts, args = getopt.getopt(sys.argv[1:],"i:f:s:d:")
    except getopt.GetoptError as e:
        print (str(e))
        print("Usage: %s -i input [-d symbol_dictionary_filename]" % sys.argv[0])
        sys.exit(2)     

    for o, a in myopts:
        if o == '-i':
            ifile=a
        elif o == '-d':
            dfile=a
        else:
            print("Usage: %s -i input [-d symbol_dictionary_filename]" % sys.argv[0])
        
    if ifile == '':
        print "No input file provided."
        print("Usage: %s -i input [-d symbol_dictionary_filename]" % sys.argv[0])
        sys.exit(2)
        
    print ("Input file : %s\nSymbolDictionary file:%s" % (ifile,dfile) )    
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
    create_files(dfile)
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