using System.Globalization;

namespace Asm6502
{
    partial class Scanner
    {
        public Token Scan()
        {
            //** let ws = [ \a\b\f\r\t\v]+
            //** let number = [0-9]+
            //** let hex = \$[0-9A-Fa-f]+
            //** let ident = [A-Z_a-z][0-9A-Z_a-z]*
            //** let string = \"(\\(.|\n)|[^\\\"\n])*(\"|\n)
            //** let char = \'.
            //** let remark = ;.*

            //** ADC
            return Token.ADC;
            //** AND
            return Token.AND;
            //** ASL
            return Token.ASL;
            //** BCC
            return Token.BCC;
            //** BCS
            return Token.BCS;
            //** BEQ
            return Token.BEQ;
            //** BIT
            return Token.BIT;
            //** BMI
            return Token.BMI;
            //** BNE
            return Token.BNE;
            //** BPL
            return Token.BPL;
            //** BRK
            return Token.BRK;
            //** BVC
            return Token.BVC;
            //** BVS
            return Token.BVS;
            //** CLC
            return Token.CLC;
            //** CLD
            return Token.CLD;
            //** CLI
            return Token.CLI;
            //** CLV
            return Token.CLV;
            //** CMP
            return Token.CMP;
            //** CPX
            return Token.CPX;
            //** CPY
            return Token.CPY;
            //** DEC
            return Token.DEC;
            //** DEX
            return Token.DEX;
            //** DEY
            return Token.DEY;
            //** EOR
            return Token.EOR;
            //** INC
            return Token.INC;
            //** INX
            return Token.INX;
            //** INY
            return Token.INY;
            //** JMP
            return Token.JMP;
            //** JSR
            return Token.JSR;
            //** LDA
            return Token.LDA;
            //** LDX
            return Token.LDX;
            //** LDY
            return Token.LDY;
            //** LSR
            return Token.LSR;
            //** NOP
            return Token.NOP;
            //** ORA
            return Token.ORA;
            //** PHA
            return Token.PHA;
            //** PHP
            return Token.PHP;
            //** PLA
            return Token.PLA;
            //** PLP
            return Token.PLP;
            //** ROL
            return Token.ROL;
            //** ROR
            return Token.ROR;
            //** RTI
            return Token.RTI;
            //** RTS
            return Token.RTS;
            //** SBC
            return Token.SBC;
            //** SEC
            return Token.SEC;
            //** SED
            return Token.SED;
            //** SEI
            return Token.SEI;
            //** STA
            return Token.STA;
            //** STX
            return Token.STX;
            //** STY
            return Token.STY;
            //** TAX
            return Token.TAX;
            //** TAY
            return Token.TAY;
            //** TSX
            return Token.TSX;
            //** TXA
            return Token.TXA;
            //** TXS
            return Token.TXS;
            //** TYA
            return Token.TYA;
            //** {ident}
            value.String= yy.tokenValue;
            return Token.Identifier;
            //** {number}
            value.Number= int.Parse(yy.tokenValue);
            return Token.Number;
            //** {hex}
            value.Number= int.Parse(yy.tokenValue.Substring(1), NumberStyles.AllowHexSpecifier);
            return Token.Number;
            //** {char}
            value.Number= MapToPetscii(yy.tokenValue[1]);
            return Token.Number;
            //** {remark}
            // Ignore remarks.
            //** {ws}
            // Ignore whitespace.
            //**
            if(yy.ScanValue == '\n')
                ++LineNumber;
            return (Token)yy.tokenValue[0];
        }
    }
}
