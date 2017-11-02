using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyScanner
{
    public class ScannerPhase
    {
        List<KeyValuePair<string, string>> symboles = new List<KeyValuePair<string, string>>();
        //private Dictionary<string, string> symboles = new Dictionary<string, string>();
        private List<int> lineNumber = new List<int>();

        enum TokenType
        {
            Datatype, ReservedKeyWords, Number, Char, Comment, Error, Identifier, String, Main
        }
        enum Datatype
        {
            Int, Float, String, Char
        }

        enum ReservedKeyWordes
        {
            Read, Write, Repeat, Until, If, Elseif, Else, Then, Return, Endl, Main, End
        }

        public List<int> getlineNumberList()
        {
            return lineNumber;
        }

        private void setSymboles()
        {
            symboles.Add(new KeyValuePair<string, string>("+", "Plus"));
            symboles.Add(new KeyValuePair<string, string>("-", "Minus"));
            symboles.Add(new KeyValuePair<string, string>("/", "Division"));
            symboles.Add(new KeyValuePair<string, string>("*", "Times"));

            symboles.Add(new KeyValuePair<string, string>(">", "GreaterThan"));
            symboles.Add(new KeyValuePair<string, string>("<", "LessThan"));
            symboles.Add(new KeyValuePair<string, string>(">=", "GreaterThanOrEqual"));
            symboles.Add(new KeyValuePair<string, string>("<=", "LessThanOrEqual"));
            symboles.Add(new KeyValuePair<string, string>("=","Equal"));
            symboles.Add(new KeyValuePair<string, string>(":=", "Assign"));
                    
            symboles.Add(new KeyValuePair<string, string>(")", "RightParentheses"));
            symboles.Add(new KeyValuePair<string, string>("(", "LeftParentheses"));

            symboles.Add(new KeyValuePair<string, string>(";", "SemiColon"));
            symboles.Add(new KeyValuePair<string, string>("}", "RightBraces"));
            symboles.Add(new KeyValuePair<string, string>("{", "LeftBraces"));
            symboles.Add(new KeyValuePair<string, string>(",", "Comma"));
            symboles.Add(new KeyValuePair<string, string>("||", "Or"));
            symboles.Add(new KeyValuePair<string, string>("&&", "And"));
            
        }

        private String getSymbolName(string symbol)
        {
            foreach (var aSymbol in symboles)
            {
                if (aSymbol.Key == symbol)
                    return aSymbol.Value;
            }
            return "";
        }

        public void scanning(string[] sourceCode, ref List<KeyValuePair<string, string>> scannedCode ) {
            setSymboles();
            string tokenType = "", lexeme="";
            bool comment = false, error=false, stringQWithoutQuate=false;
            int cnt = 0;
            foreach (string line in sourceCode)
            {
                cnt++;
                if (line.Length > 0)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        //Handle: String
                        if (line[i] == '\"' && i + 1 < line.Length && !comment)
                        {
                            lexeme = "";
                            lexeme += line[i];
                            i++;
                            while ( i < line.Length && line[i] != '\"')
                            {
                                lexeme += line[i];
                                if(i==line.Length-1 && line[i]!='\"')
                                {
                                    stringQWithoutQuate = true;
                                    break;
                                }
                                i++;
                               
                            }
                            if (!stringQWithoutQuate)
                            {
                                addInScannerList(ref scannedCode, lexeme, TokenType.String.ToString());
                                continue;
                            }
                            else
                            {
                                addInScannerList(ref scannedCode, lexeme, TokenType.Error.ToString());
                                stringQWithoutQuate = false;
                                lineNumber.Add(cnt);
                                continue;
                            }
                        }
                        //Handle: comment
                        if (comment)
                        {
                            if (i + 1 < line.Length && line[i] == '*' && line[i + 1] == '/')
                            {
                                lexeme += "*/";
                                comment = false;
                                tokenType = TokenType.Comment.ToString();
                                addInScannerList(ref scannedCode, lexeme, tokenType);
                                i++;
                            }
                            else
                            {
                                lexeme += line[i];
                            }
                            continue;
                        }

                        else if (i + 1 < line.Length && line[i] == '/' && line[i+1] == '*')
                        {
                            lexeme += "/*";
                            comment = true;
                            continue;
                        }

                        //Handle: Idetifier
                        lexeme = getIdentifier(line, ref i);
                        if (lexeme != "")
                        {
                            string c= lexeme.First().ToString();
                            lexeme=lexeme.First().ToString().ToUpper()+lexeme.Substring(1);
                            //check if this idetifier is reserved keyword
                            if (Enum.IsDefined(typeof(ReservedKeyWordes), lexeme))
                            {
                                addInScannerList(ref scannedCode, c + lexeme.Substring(1), lexeme);
                            }
                            else if (Enum.IsDefined(typeof(Datatype), lexeme))
                            {
                                addInScannerList(ref scannedCode, c + lexeme.Substring(1), "DataType");
                            }
                            else
                                addInScannerList(ref scannedCode, c+lexeme.Substring(1), TokenType.Identifier.ToString());
                            i--; //when return from getIdetifier the i is increased by 1
                            continue;
                        }

                        //Handle: character
                        if (i + 2 < line.Length && line[i] == '\'' && line[i + 2] == '\'')
                        {
                            lexeme = "";
                            lexeme = line[i + 1]+"";
                            i += 2;
                            addInScannerList(ref scannedCode, lexeme, TokenType.Char.ToString());
                            continue;

                        }

                        //Handle: number
                        error = false;
                        lexeme =getNumber(line,ref i, ref error);
                        if (lexeme != "")
                        {
                            if (error)
                            {
                                lineNumber.Add(cnt);
                                tokenType = TokenType.Error.ToString();
                            }
                            else
                                tokenType = TokenType.Number.ToString();
                            addInScannerList(ref scannedCode, lexeme, tokenType);
                            //need to sub -1 from i to start from the char after the number
                            i--;
                            continue;
                        }

                        //Handle: symbols
                        if (i < line.Length)
                        {
                            string symbol = "";
                            string symbolName = "";
                            if (i + 1 < line.Length)
                            {
                                symbol = line[i] + line[i + 1].ToString();
                                symbolName = getSymbolName(symbol);
                                if (symbolName != "")
                                {
                                    addInScannerList(ref scannedCode, symbol, symbolName);
                                    i++;
                                    continue;
                                }
                            }
                            symbol = line[i].ToString();
                            symbolName = getSymbolName(symbol);
                            if (symbolName != "" && !stringQWithoutQuate)
                            {
                                addInScannerList(ref scannedCode, symbol, symbolName);
                                continue;
                            }
                            
                        }


                        if (line[i] != ' ' && line[i] != '\t')
                        {
                            lineNumber.Add(cnt);
                            lexeme += line[i];
                            while (i + 1 < line.Length && (line[i + 1] != ' '&& line[i] != '\t'))
                            {
                                lexeme += line[++i];
                            }
                            if (!stringQWithoutQuate)
                            addInScannerList(ref scannedCode, lexeme, TokenType.Error.ToString());
                        }
                        
                    }
                }
            }
            if (comment)
            {
                lineNumber.Add(cnt);
                addInScannerList(ref scannedCode, lexeme, TokenType.Error.ToString());
            }
        }

        private void addInScannerList(ref List<KeyValuePair<string, string>>scannedCode, string s, string tokenType)
        {
            scannedCode.Add(new KeyValuePair<string, string>(s, tokenType));
            //scannedCode[s]=tokenType;
        }

        private bool isChar(char c) {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

        private bool isDigit(int n) {
            return n >= '0' && n <= '9';
        }

        private bool isDot(char c)
        {
            return c=='.';
        }
    
        private string getIdentifier(string s, ref int idx) {
            string ID="";
            //first char must be character
            if(isChar(s[idx])) {
                ID +=s[idx++];
                while(idx < s.Length && (isChar(s[idx]) || isDigit(s[idx]))) {
                    ID += s[idx++];
                }
            }
            return ID;
        }

        private string getNumber(string s, ref int idx, ref bool error) {
            string number="";
            int dot=0;
            if(isDigit(s[idx])) {
                number +=s[idx++];
                while(idx < s.Length && (isDigit(s[idx]) || isDot(s[idx]) )) {
                    if(isDigit(s[idx])) {
                        number +=s[idx++];
                    } 
                    else if(isDot(s[idx])) {
                        number +=s[idx++];
                        dot++;
                    }   
                }
            }
            if (dot > 1)
                error = true;
            return number;
        }

    }
}
