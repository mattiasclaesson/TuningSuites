#
# AS2 Parser
# Version: 0.11
#
import sys
import os.path

# Debug-mode
debug = False

#
SYMBOL_NAME = 				'dict_symbol_name'
SYMBOL_SOURCE =             'dict_symbol_source'
SYMBOL_TYPE = 				'dict_symbol_type'
SYMBOL_UNIT_OF_MEASURE = 	'dict_symbol_unit_of_measure'
SYMBOL_UNIT_VAL = 			'dict_ssymbol_unit_val'
SYMBOL_DETAILS = 			'dict_symbol_details'
SYBOL_DESCRIPTION = 		'dict_symbol_description'
SYMBOL_X_AXIS =             'dict_x_axis'
SYMBOL_X_AXIS_FUNC =        'dict_x_axis_func'
SYMBOL_Y_AXIS =             'dict_y_axis'
SYMBOL_Y_AXIS_FUNC =        'dict_y_axis_func'
SYMBOL_DUPLICATE =          'dict_symbol_duplicate'
SYMBOL_DUP_NAME =           'dict_dup_name'

TYPE_SCALAR = 'SCALAR'
TYPE_MAP = 'MAP'
TYPE_TABLE = 'TABLE'
TYPE_TABLENOSP = 'TABLENOSP'

# Counters
num_SCALAR = 0
num_MAP = 0
num_TABLE = 0
num_TABLENOSP = 0
num_DUP = 0
symbol_list = []
f_versions = ''

def create_files(out_sym_dict_file, source_file):
    in_sym_dict = """using System;
using System.Collections.Generic;
namespace T8SuitePro
{
    class MySymbol
    {
        public string source;
        public string type;
        public double unitVal;
        public string unitOfMeasure;
        public string description;
        public string xAxis;
        public string xAxisFunction;
        public string yAxis;
        public string yAxisFunction;
        public string duplicateName;
        public bool duplicateExist;
        
        public MySymbol(string inSource, string inType, double inUnitVal, string inUnitOfMeasure, 
                        string inDescription, string inXAxis, string inXAxisFunction, 
                        string inYAxis, string inYAxisFunction, string inDuplicateName, bool inDuplicateExist)
        {
            source = inSource;
            type = inType;
            unitVal = inUnitVal;
            unitOfMeasure = inUnitOfMeasure;
            description = inDescription;
            xAxis = inXAxis;
            xAxisFunction = inXAxisFunction;
            yAxis = inYAxis;
            yAxisFunction = inYAxisFunction;
            duplicateName = inDuplicateName;
            duplicateExist = inDuplicateExist;
        }
    }
    static class SymbolDictionary
    {
        static Dictionary<string, MySymbol> _dict = new Dictionary<string, MySymbol>
        {"""
            
            
    out_sym_dict = """
            {"END", new MySymbol("", "",0,"","", "","","","", "", false)}
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
                return 1.0;
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
        public static bool doesDuplicateExist(string word, out char ax_type, out string alt_axis)
        {
            MySymbol result;
            ax_type = ' ';
            alt_axis = "";
            if (_dict.TryGetValue(word, out result))
            {
                // E.g. "X.BstKnkCal.OffsetXSP"
                if (result.duplicateExist)
                {
                    ax_type = (char)result.duplicateName[0];
                    alt_axis = result.duplicateName.Substring(2);
                }
                return result.duplicateExist;
            }
            else
            {
                return false;
            }            
        }
        public static string returnOldSoftware()
        {
"""
    out_sym_dict_tail = """
            if (usedVersions.Length > 1)
                return usedVersions[1];
            return "";
        }
    }
}"""

    # Heading
    fh_sym_dic = open(out_sym_dict_file, 'w')    
    fh_sym_dic.write("//\n// WARNING WARNING WARNING\n//\n// This is a GENERATED file, do not edit.\n// Run as2parse.py in Utils directory instead!!!\n")
    fh_sym_dic.write("//\n// Source of symbols: " + source_file + "\n//\n//\n")
    fh_sym_dic.write(in_sym_dict)
    
    # Repetitive
    for a_symbol in symbol_list:
        fh_sym_dic.write('\n            {"' + 
                            a_symbol[SYMBOL_NAME] + '\", new MySymbol(\"' +\
                            a_symbol[SYMBOL_SOURCE] +             '\", \"' +\
                            a_symbol[SYMBOL_TYPE] +               '\", ' +\
                            str(a_symbol[SYMBOL_UNIT_VAL]) +           ', \"' +\
                            a_symbol[SYMBOL_UNIT_OF_MEASURE] +    '\", \"' +\
                            a_symbol[SYBOL_DESCRIPTION] +         '\", \"' +\
                            a_symbol[SYMBOL_X_AXIS] +             '\", \"' +\
                            a_symbol[SYMBOL_X_AXIS_FUNC] +        '\", \"' +\
                            a_symbol[SYMBOL_Y_AXIS] +             '\", \"' +\
                            a_symbol[SYMBOL_Y_AXIS_FUNC] +        '\", \"' +\
                            a_symbol[SYMBOL_DUP_NAME] +           '\", ' +\
                            a_symbol[SYMBOL_DUPLICATE] +          ')},')
    # Tail
    fh_sym_dic.write(out_sym_dict)
    #"            string[] usedVersions = new string[] { "FF0L", "FC01"};"
    fh_sym_dic.write("            string[] usedVersions = new string[] {" + f_versions + "};")
    fh_sym_dic.write(out_sym_dict_tail)
    fh_sym_dic.close()

def symbol_exist_already(symbol_name, the_symbol):
    global num_DUP
    for a_symbol in symbol_list:
        if a_symbol[SYMBOL_NAME] == symbol_name:
            num_DUP = num_DUP + 1
            if a_symbol[SYMBOL_TYPE] != the_symbol[SYMBOL_TYPE]:
                print "Found type mismatch for map '" + symbol_name + "'"
                return "TypeMismatch"
            if (a_symbol[SYMBOL_X_AXIS] != the_symbol[SYMBOL_X_AXIS]):
                a_symbol[SYMBOL_DUPLICATE] = "true"
                the_symbol[SYMBOL_DUPLICATE] = "true"
                a_symbol[SYMBOL_DUP_NAME] = "X." + the_symbol[SYMBOL_X_AXIS]
                the_symbol[SYMBOL_DUP_NAME] = "X." + a_symbol[SYMBOL_X_AXIS]
                #print symbol_name
                print "Found x-axis mismatch for map '" + symbol_name + "'"
                print "File A: '" + a_symbol[SYMBOL_X_AXIS] + "'"
                print "File B: '" + the_symbol[SYMBOL_X_AXIS] + "'"
                return "YesSaveDup"
            if (a_symbol[SYMBOL_Y_AXIS] != the_symbol[SYMBOL_Y_AXIS]):
                a_symbol[SYMBOL_DUPLICATE] = "true"
                the_symbol[SYMBOL_DUPLICATE] = "true"
                a_symbol[SYMBOL_DUP_NAME] = "Y." + the_symbol[SYMBOL_Y_AXIS]
                the_symbol[SYMBOL_DUP_NAME] = "Y." + a_symbol[SYMBOL_Y_AXIS]
                #print symbol_name
                print "Found x-axis mismatch for map '" + symbol_name + "'"
                print "File A: '" + a_symbol[SYMBOL_Y_AXIS] + "'"
                print "File B: '" + the_symbol[SYMBOL_Y_AXIS] + "'"
                return "YesSaveDup"
            if debug:
                print "Found duplicated symbol " + symbol_name
            return "YesNoSave"
    
    return "No"
    
def parse_symbol_data(symbol_name, symbol_source, symbol_data):
    global num_SCALAR
    global num_MAP
    global num_TABLE
    global num_TABLENOSP
    
    my_symbol = {};
    my_symbol[SYMBOL_NAME] = symbol_name
    my_symbol[SYMBOL_SOURCE] = symbol_source
    my_symbol[SYMBOL_X_AXIS] = ""
    my_symbol[SYMBOL_X_AXIS_FUNC] = ""
    my_symbol[SYMBOL_Y_AXIS] = ""
    my_symbol[SYMBOL_Y_AXIS_FUNC] = ""
    my_symbol[SYBOL_DESCRIPTION] = ""
    my_symbol[SYMBOL_DUPLICATE] = "false"
    my_symbol[SYMBOL_DUP_NAME] = ""
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
    global num_DUP
    global symbol_list
    dfile='SymbolDictionary.cs'
    global f_versions

    if len(sys.argv) < 2:
        print("Usage: %s as2_input_1 [as2_input_n*]" % sys.argv[0])
        exit(1)
        
    for idx, arg in enumerate(sys.argv):
        if idx != 0:
            if not os.path.exists(arg):
                print "Input .as2 file '%s' does not exist" % arg
                exit(1)
             
            print "Input file : %s" % arg   
            fh = open(arg)
    
            # Read headers
            file_version = fh.readline().strip()
            symbols = int(fh.readline().split(':')[1].strip())
            
            if idx == 1:
                f_versions = '\"' + file_version + '\"'
            else:
                f_versions = f_versions + ", " + '\"' + file_version + '\"'

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
                        #found_symbol = parse_symbol_data(symbol_name, file_version, symbol_data)
                        #if not symbol_exist_already(symbol_name, found_symbol):
                        #    symbol_list.append(found_symbol)
                        found_symbol = parse_symbol_data(symbol_name, file_version, symbol_data)
                        dup_found = symbol_exist_already(symbol_name, found_symbol)
                        if dup_found == "YesSaveDup":
                            # A duplicate that needs to be stored found, rename it and add trailing source information
                            found_symbol[SYMBOL_NAME] = found_symbol[SYMBOL_NAME] + "." + found_symbol[SYMBOL_SOURCE]
                            symbol_list.append(found_symbol)
                        elif dup_found == "No":
                            symbol_list.append(found_symbol)
                        elif dup_found == "YesNoSave":
                            # Do nothing
                            if debug:
                                print "Found a duplicate, will not save it though"
                        
                        searching = True
                        symbol_data = []
                    else:
                        symbol_data.append(line)
            fh.close()
 
    # Create output files
    create_files(dfile, f_versions)
    print "Parsed %s symbols successfully" % str(num_SCALAR + num_MAP + num_TABLE + num_TABLENOSP)

    # Check file consistency
    if symbols != num_SCALAR + num_MAP + num_TABLE + num_TABLENOSP:
        print("Mismatch!")
        print("Number of symbols to be found:" + str(symbols))
        print("Number of symbols found: " + str(num_SCALAR + num_MAP + num_TABLE + num_TABLENOSP))
        print("Number of duplicates found: " + str(num_DUP))
        print(" - SCALAR:  " + str(num_SCALAR))
        print(" - MAP:     " + str(num_MAP))
        print(" - TABLE:   " + str(num_TABLE))
        print(" - TABLESP: " + str(num_TABLENOSP))
    if debug:
        print symbol_list
        
if __name__ == '__main__':
    main()