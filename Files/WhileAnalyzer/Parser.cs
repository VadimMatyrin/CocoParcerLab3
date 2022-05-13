
using System;

namespace WhileAnalyzer {



public class Parser {
	public const int _EOF = 0;
	public const int _number = 1;
	public const int _ident = 2;
	public const int _string = 3;
	public const int _relOp = 4;
	public const int _mathSymb = 5;
	public const int _while = 6;
	public const int _semicol = 7;
	public const int maxT = 13;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Sample() {
		while (la.kind == 6) {
			Expr();
		}
	}

	void Expr() {
		Expect(6);
		Expect(8);
		boolean();
		Expect(9);
		body();
	}

	void boolean() {
		if (la.kind == 2) {
			Get();
		} else if (la.kind == 1) {
			Get();
		} else SynErr(14);
		Expect(4);
		if (la.kind == 2) {
			Get();
		} else if (la.kind == 1) {
			Get();
		} else SynErr(15);
	}

	void body() {
		if (la.kind == 10) {
			Get();
			statement();
			while (la.kind == 2) {
				statement();
			}
			Expect(11);
		} else if (la.kind == 2) {
			statement();
		} else SynErr(16);
	}

	void statement() {
		Expect(2);
		Expect(12);
		if (la.kind == 1 || la.kind == 8) {
			math();
		} else if (la.kind == 3) {
			Get();
		} else if (la.kind == 1) {
			Get();
		} else if (la.kind == 2) {
			Get();
		} else SynErr(17);
		if (la.kind == 7) {
			Get();
		}
	}

	void math() {
		if (la.kind == 8) {
			Get();
			Expect(1);
			while (la.kind == 5) {
				Get();
				if (la.kind == 1 || la.kind == 8) {
					math();
				} else if (la.kind == 1) {
					Get();
				} else SynErr(18);
			}
			Expect(9);
		} else if (la.kind == 1) {
			Get();
			while (la.kind == 5) {
				Get();
				if (la.kind == 1 || la.kind == 8) {
					math();
				} else if (la.kind == 1) {
					Get();
				} else SynErr(19);
			}
		} else SynErr(20);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Sample();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "number expected"; break;
			case 2: s = "ident expected"; break;
			case 3: s = "string expected"; break;
			case 4: s = "relOp expected"; break;
			case 5: s = "mathSymb expected"; break;
			case 6: s = "while expected"; break;
			case 7: s = "semicol expected"; break;
			case 8: s = "\"(\" expected"; break;
			case 9: s = "\")\" expected"; break;
			case 10: s = "\"{\" expected"; break;
			case 11: s = "\"}\" expected"; break;
			case 12: s = "\"=\" expected"; break;
			case 13: s = "??? expected"; break;
			case 14: s = "invalid boolean"; break;
			case 15: s = "invalid boolean"; break;
			case 16: s = "invalid body"; break;
			case 17: s = "invalid statement"; break;
			case 18: s = "invalid math"; break;
			case 19: s = "invalid math"; break;
			case 20: s = "invalid math"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}