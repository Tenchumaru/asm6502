using System.IO;
using System.Text;

namespace Asm6502
{
    enum Token
    {
        EOF = -1, None, NewLine = '\n', Splat = '*', Equals = '=', Colon = ':',
        Plus = '+', Minus = '-', Octothorpe = '#', OP = '(', CP = ')', Comma = ',',
        Identifier = 0x100, Number,
        ADC, AND, ASL, BCC, BCS, BEQ, BIT, BMI, BNE, BPL, BRK, BVC, BVS, CLC,
        CLD, CLI, CLV, CMP, CPX, CPY, DEC, DEX, DEY, EOR, INC, INX, INY, JMP,
        JSR, LDA, LDX, LDY, LSR, NOP, ORA, PHA, PHP, PLA, PLP, ROL, ROR, RTI,
        RTS, SBC, SEC, SED, SEI, STA, STX, STY, TAX, TAY, TSX, TXA, TXS, TYA,
        OpcodeOffset = ADC
    }

    class LexicalValue
    {
        public int Number;
        public string String;
    }

    partial class Scanner
    {
        public int LineNumber { get; private set; }
        private LexicalValue value = new LexicalValue();
        private TextReader reader;
        private StringBuilder buffer = new StringBuilder();
        private int marker, position;
        private string tokenValue;
        private Scanner yy;
        private int ScanValue;

        public Scanner(TextReader reader)
        {
            this.reader = reader;
            yy = this;
            LineNumber = 1;
        }

        private static int MapToPetscii(char ch)
        {
            // TODO
            return ch;
        }

        internal int Number
        {
            get { return value.Number; }
        }

        internal string String
        {
            get { return value.String.ToUpperInvariant(); }
        }

        private void Save()
        {
            marker = position;
        }

        private void Restore()
        {
            tokenValue = buffer.ToString(0, marker);
            buffer.Remove(position = 0, marker);
            marker = 0;
        }

        private int Get()
        {
            if(position >= buffer.Length)
            {
                if(ScanValue < 0)
                    return ScanValue;
                int ch = reader.Read();
                if(ch < 0)
                    return ScanValue = -1;
                buffer.Append((char)ch);

            }
            ++position;
            return ScanValue = buffer[position - 1];
        }
    }
}
