using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Asm6502
{
    partial class Parser
    {
        private enum Mode
        {
            Implied, Immediate, ZeroPage, ZeroPageX, ZeroPageY,
            Absolute, AbsoluteX, AbsoluteY, IndirectX, IndirectY, Indirect
        }

        private static readonly short[,] assemblerData = {
            //         IMP   IMM   ZP    ZPX   ZPY   ABS   ABX   ABY   INX   INY   IND
            /*ADC*/ {   -1, 0x69, 0x65, 0x75,   -1, 0x6d, 0x7d, 0x79, 0x61, 0x71,   -1 },
            /*AND*/ {   -1, 0x29, 0x25, 0x35,   -1, 0x2d, 0x3d, 0x39, 0x21, 0x31,   -1 },
            /*ASL*/ { 0x0a,   -1, 0x06, 0x16,   -1, 0x0e, 0x1e,   -1,   -1,   -1,   -1 },
            /*BCC*/ {   -1,   -1,   -1,   -1,   -1, 0x90,   -1,   -1,   -1,   -1,   -1 },
            /*BCS*/ {   -1,   -1,   -1,   -1,   -1, 0xb0,   -1,   -1,   -1,   -1,   -1 },
            /*BEQ*/ {   -1,   -1,   -1,   -1,   -1, 0xf0,   -1,   -1,   -1,   -1,   -1 },
            /*BIT*/ {   -1,   -1, 0x24,   -1,   -1, 0x2c,   -1,   -1,   -1,   -1,   -1 },
            /*BMI*/ {   -1,   -1,   -1,   -1,   -1, 0x30,   -1,   -1,   -1,   -1,   -1 },
            /*BNE*/ {   -1,   -1,   -1,   -1,   -1, 0xd0,   -1,   -1,   -1,   -1,   -1 },
            /*BPL*/ {   -1,   -1,   -1,   -1,   -1, 0x10,   -1,   -1,   -1,   -1,   -1 },
            /*BRK*/ { 0x00,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*BVC*/ {   -1,   -1,   -1,   -1,   -1, 0x50,   -1,   -1,   -1,   -1,   -1 },
            /*BVS*/ {   -1,   -1,   -1,   -1,   -1, 0x70,   -1,   -1,   -1,   -1,   -1 },
            /*CLC*/ { 0x18,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*CLD*/ { 0xd8,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*CLI*/ { 0x58,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*CLV*/ { 0xb8,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*CMP*/ {   -1, 0xc9, 0xc5, 0xd5,   -1, 0xcd, 0xdd, 0xd9, 0xc1, 0xd1,   -1 },
            /*CPX*/ {   -1, 0xe0, 0xe4,   -1,   -1, 0xec,   -1,   -1,   -1,   -1,   -1 },
            /*CPY*/ {   -1, 0xc0, 0xc4,   -1,   -1, 0xcc,   -1,   -1,   -1,   -1,   -1 },
            /*DEC*/ {   -1,   -1, 0xc6, 0xd6,   -1, 0xce, 0xde,   -1,   -1,   -1,   -1 },
            /*DEX*/ { 0xca,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*DEY*/ { 0x88,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*EOR*/ {   -1, 0x49, 0x45, 0x55,   -1, 0x4d, 0x5d, 0x59, 0x41, 0x51,   -1 },
            /*INC*/ {   -1,   -1, 0xe6, 0xf6,   -1, 0xee, 0xfe,   -1,   -1,   -1,   -1 },
            /*INX*/ { 0xe8,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*INY*/ { 0xb8,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*JMP*/ {   -1,   -1,   -1,   -1,   -1, 0x4c,   -1,   -1,   -1,   -1, 0x6c },
            /*JSR*/ {   -1,   -1,   -1,   -1,   -1, 0x20,   -1,   -1,   -1,   -1,   -1 },
            /*LDA*/ {   -1, 0xa9, 0xa5, 0xb5,   -1, 0xad, 0xbd, 0xb9, 0xa1, 0xb1,   -1 },
            /*LDX*/ {   -1, 0xa2, 0xa6,   -1, 0xb6, 0xae,   -1, 0xbe,   -1,   -1,   -1 },
            /*LDY*/ {   -1, 0xa0, 0xa4, 0xb4,   -1, 0xac, 0xbc,   -1,   -1,   -1,   -1 },
            /*LSR*/ { 0x4a,   -1, 0x46, 0x56,   -1, 0x4e, 0x5e,   -1,   -1,   -1,   -1 },
            /*NOP*/ { 0xea,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*ORA*/ {   -1, 0x09, 0x05, 0x15,   -1, 0x0d, 0x1d, 0x19, 0x01, 0x11,   -1 },
            /*PHA*/ { 0x48,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*PHP*/ { 0x08,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*PLA*/ { 0x68,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*PLP*/ { 0x28,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*ROL*/ { 0x2a,   -1, 0x26, 0x36,   -1, 0x2e, 0x3e,   -1,   -1,   -1,   -1 },
            /*ROR*/ { 0x6a,   -1, 0x66, 0x76,   -1, 0x6e, 0x7e,   -1,   -1,   -1,   -1 },
            /*RTI*/ { 0x40,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*RTS*/ { 0x60,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*SBC*/ {   -1, 0xe9, 0xe5, 0xf5,   -1, 0xed, 0xfd, 0xf9, 0xe1, 0xf1,   -1 },
            /*SEC*/ { 0x38,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*SED*/ { 0xf8,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*SEI*/ { 0x78,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*STA*/ {   -1,   -1, 0x85, 0x95,   -1, 0x8d, 0x9d, 0x99, 0x81, 0x91,   -1 },
            /*STX*/ {   -1,   -1, 0x86,   -1, 0x96, 0x8e,   -1,   -1,   -1,   -1,   -1 },
            /*STY*/ {   -1,   -1, 0x84, 0x94,   -1, 0x8c,   -1,   -1,   -1,   -1,   -1 },
            /*TAX*/ { 0xaa,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*TAY*/ { 0xa8,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*TSX*/ { 0xba,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*TXA*/ { 0x8a,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*TXS*/ { 0x9a,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 },
            /*TYA*/ { 0x98,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1 }
        };
        private Token lastToken = Token.None;
        private Scanner scanner;
        private byte[] program = new byte[0x10000];
        private int pc;
        private bool hasFailed, containsUnknownLabel;
        private Dictionary<string, int> labelValues = new Dictionary<string, int>();
        private Dictionary<string, List<int>> unknownLabels = new Dictionary<string, List<int>>();
        private Dictionary<string, int> entryPoints = new Dictionary<string, int>();

        internal Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

        internal byte[] GetMinimalFullSpan(out int startAddress)
        {
            for(startAddress = 0; startAddress < program.Length; ++startAddress)
            {
                if(program[startAddress] != 0)
                    break;
            }
            int endAddress;
            for(endAddress = program.Length; startAddress < endAddress; --endAddress)
            {
                if(program[endAddress - 1] != 0)
                    break;
            }
            byte[] span = new byte[endAddress - startAddress];
            Array.Copy(program, startAddress, span, 0, span.Length);
            return span;
        }

        internal bool Parse()
        {
            hasFailed = false;
            while(this.ParseLine())
                continue;
            if(unknownLabels.Count == 0)
                return !hasFailed;
            return this.ReportError("unknown labels");
        }

        private bool ParseLine()
        {
            containsUnknownLabel = false;
            Token token = this.Scan();
            switch(token)
            {
            case Token.NewLine:
                return true;
            case Token.EOF:
                return false;
            case Token.Identifier:
                token = this.Scan();
                if(token == Token.Equals)
                {
                    int value;
                    if(!this.ParseExpression(out value))
                        return false;
                    labelValues.Add(scanner.String, value);
                }
                else
                {
                    if(token == Token.Colon)
                        entryPoints.Add(scanner.String, pc);
                    else
                        this.PushBack(token);
                    List<int> unknownLabelPositions;
                    if(unknownLabels.TryGetValue(scanner.String, out unknownLabelPositions))
                    {
                        foreach(int unknownPosition in unknownLabelPositions)
                        {
                            byte instruction = program[unknownPosition - 1];
                            if(instruction == 0x20 || instruction == 0x4c)
                            {
                                program[unknownPosition] = (byte)pc;
                                program[unknownPosition + 1] = (byte)(pc >> 8);
                            }
                            else
                            {
                                // Assume it's a branch.
                                program[unknownPosition] = (byte)(pc - unknownPosition - 1);
                            }
                        }
                        unknownLabels.Remove(scanner.String);
                    }
                    labelValues.Add(scanner.String, pc);
                }
                return true;
            case Token.Splat:
                if(this.Scan() == Token.Equals)
                {
                    int value;
                    if(this.ParseExpression(out value))
                    {
                        pc = value;
                        return true;
                    }
                }
                return false;
            default:
                if(token >= Token.OpcodeOffset)
                {
                    Mode mode;
                    int value;
                    if(!this.ParseAddress(out mode, out value))
                        return false;
                    short instruction = assemblerData[token - Token.OpcodeOffset, (int)mode];
                    if(instruction == -1)
                    {
                        if(IsZeroPageMode(mode))
                            mode = (Mode)(mode + 3);
                        instruction = assemblerData[token - Token.OpcodeOffset, (int)mode];
                        if(instruction == -1)
                            return this.ReportError("illegal address");
                    }
                    program[pc++] = (byte)instruction;
                    if(IsBranch(instruction))
                    {
                        if(!containsUnknownLabel)
                            value -= pc + 1; // relative
                        mode = Mode.ZeroPage;
                    }
                    if(mode != Mode.Implied)
                    {
                        program[pc++] = (byte)value;
                        if(!IsByteSizeMode(mode))
                            program[pc++] = (byte)(value >> 8);
                        else if(!IsByteSized(value))
                            return this.ReportError("value out of range");
                    }
                    return this.Scan() == Token.NewLine;
                }
                return false;
            }
        }

        private static bool IsBranch(short instruction)
        {
            return (instruction & 0x1f) == 0x10;
        }

        private static bool IsByteSized(int value)
        {
            return value <= byte.MaxValue && value >= sbyte.MinValue;
        }

        private static bool IsByteSizeMode(Mode mode)
        {
            return mode == Mode.Immediate || IsZeroPageMode(mode)
                || mode == Mode.IndirectX || mode == Mode.IndirectY;
        }

        private static bool IsZeroPageMode(Mode mode)
        {
            return mode >= Mode.ZeroPage && mode <= Mode.ZeroPageY;
        }

        private bool ParseAddress(out Mode mode, out int value)
        {
            mode = Mode.Implied;
            value = 0;
            Token token = this.Scan();
            switch(token)
            {
            case Token.EOF:
            case Token.NewLine:
                // implied (or accumulator)
                PushBack(Token.NewLine);
                return true;
            case Token.OP:
                if(!this.ParseExpression(out value))
                    return false;
                token = this.Scan();
                switch(token)
                {
                case Token.Comma:
                    if(this.Scan() != Token.Identifier)
                        return false;
                    if(scanner.String != "X")
                        return false;
                    if(this.Scan() != Token.CP)
                        return false;
                    if(!IsByteSized(value))
                        return this.ReportError("address out of range");
                    mode = Mode.IndirectX;
                    return true;
                case Token.CP:
                    token = this.Scan();
                    if(token != Token.Comma)
                    {
                        this.PushBack(token);
                        mode = Mode.Indirect;
                        return true;
                    }
                    if(this.Scan() != Token.Identifier)
                        return false;
                    if(scanner.String != "Y")
                        return false;
                    if(!IsByteSized(value))
                        return this.ReportError("address out of range");
                    mode = Mode.IndirectY;
                    return true;
                }
                break;
            case Token.Octothorpe:
                if(!this.ParseExpression(out value))
                    return false;
                if(!IsByteSized(value))
                    return this.ReportError("value out of range");
                mode = Mode.Immediate;
                return true;
            case Token.Identifier:
            case Token.Number:
                this.PushBack(token);
                if(!this.ParseExpression(out value))
                    return false;
                token = this.Scan();
                if(token != Token.Comma)
                {
                    this.PushBack(token);
                    mode = IsByteSized(value) ? Mode.ZeroPage : Mode.Absolute;
                }
                else
                {
                    if(this.Scan() != Token.Identifier)
                        return false;
                    switch(scanner.String)
                    {
                    case "X":
                        mode = IsByteSized(value) ? Mode.ZeroPageX : Mode.AbsoluteX;
                        break;
                    case "Y":
                        mode = IsByteSized(value) ? Mode.ZeroPageY : Mode.AbsoluteY;
                        break;
                    default:
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool ParseExpression(out int value)
        {
            if(!this.ParseTerm(out value))
                return false;
            for(; ; )
            {
                Token token = this.Scan();
                if(token != Token.Minus && token != Token.Plus)
                {
                    this.PushBack(token);
                    return true;
                }
                int signum = token == Token.Minus ? -1 : 1;
                int nextValue;
                if(!this.ParseTerm(out nextValue))
                    return false;
                value += signum * nextValue;
            }
        }

        private bool ParseTerm(out int value)
        {
            return this.ParseSimple(out value);
        }

        private bool ParseSimple(out int value)
        {
            value = 0;
            Token token = this.Scan();
            switch(token)
            {
            case Token.Splat:
                value = pc;
                return true;
            case Token.Identifier:
                if(!labelValues.TryGetValue(scanner.String, out value))
                {
                    containsUnknownLabel = true;
                    List<int> unknownLabelPositions;
                    if(!unknownLabels.TryGetValue(scanner.String, out unknownLabelPositions))
                        unknownLabels.Add(scanner.String, unknownLabelPositions = new List<int>());
                    unknownLabelPositions.Add(pc + 1);
                }
                return true;
            case Token.Number:
                value = scanner.Number;
                return true;
            }
            this.PushBack(token);
            return false;
        }

        private void PushBack(Token token)
        {
            Debug.Assert(lastToken == Token.None);
            lastToken = token;
        }

        private bool ReportError(string message)
        {
            Console.Error.WriteLine("line {0}: {1}", scanner.LineNumber, message);
            hasFailed = true;
            return false;
        }

        private Token Scan()
        {
            if(lastToken != Token.None)
            {
                Token token = lastToken;
                lastToken = Token.None;
                return token;
            }
            return scanner.Scan();
        }

        internal Dictionary<string, int> EntryPoints
        {
            get { return entryPoints; }
        }
    }
}
