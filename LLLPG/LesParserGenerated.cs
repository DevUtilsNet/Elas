using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loyc;
using Loyc.LLParserGenerator;
using Loyc.Collections;
using Loyc.Syntax;
using Loyc.Syntax.Lexing;

namespace Loyc.Syntax.Les
{
	using TT = TokenType;
	using S = CodeSymbols;
	using P = LesPrecedence;

	public partial class LesParser
	{
		LNode Atom(Precedence contextA, ref RWList<LNode> attrs)
		{
			TT la0, la1;
			LNode e = F._Missing, _;
			// Line 0: ( TT.Id (&{t.EndIndex == LT($LI).StartIndex && contextA.CanParse(P.Primary)} TT.LParen TT.RParen / ) | (TT.Number|TT.Symbol|TT.String|TT.OtherLit|TT.SQString) | TT.At TT.LBrack TT.RBrack | (TT.PreSufOp|TT.PrefixOp) Expr | &{contextA != P_SuperExpr} (TT.Assignment|TT.Colon|TT.BQString|TT.Not|TT.NormalOp|TT.Dot) Expr | TT.LBrack TT.RBrack Atom | TT.LParen TT.RParen | TT.LBrace TT.RBrace )
			switch (LA0) {
			case TT.Id:
				{
					var t = MatchAny();
					// Line 0: (&{t.EndIndex == LT($LI).StartIndex && contextA.CanParse(P.Primary)} TT.LParen TT.RParen / )
					la0 = LA0;
					if (la0 == TT.LParen) {
						if (t.EndIndex == LT(0).StartIndex && contextA.CanParse(P.Primary)) {
							la1 = LA(1);
							if (la1 == TT.RParen) {
								var p = MatchAny();
								var rp = MatchAny();
								e = ParseCall(t, p, rp.EndIndex);
							} else
								e = F.Id((Symbol)t.Value, t.StartIndex, t.Length)
						} else
							e = F.Id((Symbol)t.Value, t.StartIndex, t.Length)
					} else
						e = F.Id((Symbol)t.Value, t.StartIndex, t.Length)
				}
				break;
			case TT.Number:
			case TT.OtherLit:
			case TT.SQString:
			case TT.String:
			case TT.Symbol:
				{
					var t = MatchAny();
					e = F.Literal(t.Value, t.StartIndex, t.Length);
				}
				break;
			case TT.At:
				{
					Skip();
					var t = Match((int) TT.LBrack);
					var rb = Match((int) TT.RBrack);
					e = F.Literal(t.Children, t.StartIndex, rb.EndIndex - t.StartIndex);
				}
				break;
			case TT.PrefixOp:
			case TT.PreSufOp:
				{
					var t = MatchAny();
					e = Expr(PrefixPrecedenceOf(t), out _);
					e = F.Call((Symbol)t.Value, e, t.StartIndex, e.Range.EndIndex - t.StartIndex);
				}
				break;
			case TT.Assignment:
			case TT.BQString:
			case TT.Colon:
			case TT.Dot:
			case TT.NormalOp:
			case TT.Not:
				{
					Check(contextA != P_SuperExpr, "contextA != P_SuperExpr");
					var t = MatchAny();
					e = Expr(PrefixPrecedenceOf(t), out _);
					e = F.Call((Symbol)t.Value, e, t.StartIndex, e.Range.EndIndex - t.StartIndex);
				}
				break;
			case TT.LBrack:
				{
					var t = MatchAny();
					Match((int) TT.RBrack);
					attrs = AppendExprsInside(t, attrs);
					e = Atom(contextA, ref attrs);
				}
				break;
			case TT.LParen:
				{
					var t = MatchAny();
					var rp = Match((int) TT.RParen);
					e = ParseParens(t, rp.EndIndex);
				}
				break;
			case TT.LBrace:
				{
					var t = MatchAny();
					var rb = Match((int) TT.RBrace);
					e = ParseBraces(t, rb.EndIndex);
				}
				break;
			default:
				Error(0, "In rule 'Atom', expected one of: (TT.LBrace|TT.PreSufOp|TT.Symbol|TT.Colo...");
				break;
			}
			return e;
		}
		LNode Expr(Precedence context, out LNode primary)
		{
			TT la1;
			LNode e, _; Precedence prec; RWList<LNode> attrs = null; ;
			e = Atom(context, ref attrs);
			primary = e;
			var contextA = context;
			// Line 0: greedy( (TT.Assignment|TT.Colon|TT.BQString|TT.NormalOp|TT.Dot) Expr | &{context.CanParse(P.Primary)} TT.Not Expr | &{context.CanParse(SuffixPrecedenceOf(LT($LI)))} (TT.PreSufOp|TT.SuffixOp) | &{e.Range.EndIndex == LT($LI).StartIndex && context.CanParse(P.Primary)} TT.LParen TT.RParen | &{context.CanParse(P.Primary)} TT.LBrack TT.RBrack | &{context.CanParse(P_SuperExpr)} Expr greedy(Expr)* )*
			for (;;) {
				switch (LA0) {
				case TT.Assignment:
				case TT.BQString:
				case TT.Colon:
				case TT.Dot:
				case TT.NormalOp:
					{
						switch (LA(1)) {
						case TT.At:
						case TT.Id:
						case TT.Number:
						case TT.OtherLit:
						case TT.PrefixOp:
						case TT.PreSufOp:
						case TT.SQString:
						case TT.String:
						case TT.Symbol:
							goto match1;
						case TT.Assignment:
						case TT.BQString:
						case TT.Colon:
						case TT.Dot:
						case TT.NormalOp:
						case TT.Not:
							{
								if (contextA != P_SuperExpr)
									goto match1;
								else
									goto stop2;
							}
						case TT.LBrace:
						case TT.LBrack:
						case TT.LParen:
							goto match1;
						default:
							goto stop2;
						}
					}
				case TT.Not:
					{
						if (context.CanParse(P.Primary)) {
							switch (LA(1)) {
							case TT.At:
							case TT.Id:
							case TT.Number:
							case TT.OtherLit:
							case TT.PrefixOp:
							case TT.PreSufOp:
							case TT.SQString:
							case TT.String:
							case TT.Symbol:
								goto match2;
							case TT.Assignment:
							case TT.BQString:
							case TT.Colon:
							case TT.Dot:
							case TT.NormalOp:
							case TT.Not:
								{
									if (contextA != P_SuperExpr)
										goto match2;
									else
										goto stop2;
								}
							case TT.LBrace:
							case TT.LBrack:
							case TT.LParen:
								goto match2;
							default:
								goto stop2;
							}
						} else if (context.CanParse(P_SuperExpr) && contextA != P_SuperExpr) {
							switch (LA(1)) {
							case TT.At:
							case TT.Id:
							case TT.Number:
							case TT.OtherLit:
							case TT.PrefixOp:
							case TT.PreSufOp:
							case TT.SQString:
							case TT.String:
							case TT.Symbol:
								goto match6;
							case TT.Assignment:
							case TT.BQString:
							case TT.Colon:
							case TT.Dot:
							case TT.NormalOp:
							case TT.Not:
								{
									if (contextA != P_SuperExpr)
										goto match6;
									else
										goto stop2;
								}
							case TT.LBrace:
							case TT.LBrack:
							case TT.LParen:
								goto match6;
							default:
								goto stop2;
							}
						} else
							goto stop2;
					}
				case TT.PreSufOp:
					{
						if (context.CanParse(SuffixPrecedenceOf(LT(0))))
							goto match3;
						else if (context.CanParse(P_SuperExpr)) {
							switch (LA(1)) {
							case TT.At:
							case TT.Id:
							case TT.Number:
							case TT.OtherLit:
							case TT.PrefixOp:
							case TT.PreSufOp:
							case TT.SQString:
							case TT.String:
							case TT.Symbol:
								goto match6;
							case TT.Assignment:
							case TT.BQString:
							case TT.Colon:
							case TT.Dot:
							case TT.NormalOp:
							case TT.Not:
								{
									if (contextA != P_SuperExpr)
										goto match6;
									else
										goto stop2;
								}
							case TT.LBrace:
							case TT.LBrack:
							case TT.LParen:
								goto match6;
							default:
								goto stop2;
							}
						} else
							goto stop2;
					}
				case TT.SuffixOp:
					{
						if (context.CanParse(SuffixPrecedenceOf(LT(0))))
							goto match3;
						else
							goto stop2;
					}
				case TT.LParen:
					{
						if (e.Range.EndIndex == LT(0).StartIndex && context.CanParse(P.Primary)) {
							la1 = LA(1);
							if (la1 == TT.RParen) {
								var p = MatchAny();
								var rp = MatchAny();
								e = primary = ParseCall(e, p, rp.EndIndex);
								e.BaseStyle = NodeStyle.PurePrefixNotation;
							} else
								goto stop2;
						} else if (context.CanParse(P_SuperExpr)) {
							la1 = LA(1);
							if (la1 == TT.RParen)
								goto match6;
							else
								goto stop2;
						} else
							goto stop2;
					}
					break;
				case TT.LBrack:
					{
						if (context.CanParse(P.Primary)) {
							la1 = LA(1);
							if (la1 == TT.RBrack) {
								var t = MatchAny();
								var rb = MatchAny();
								
							var args = new RWList<LNode> { e };
							AppendExprsInside(t, args);
							e = primary = F.Call(S.Bracks, args.ToRVList(), e.Range.StartIndex, rb.EndIndex - e.Range.StartIndex);
							e.BaseStyle = NodeStyle.Expression;;
							} else
								goto stop2;
						} else if (context.CanParse(P_SuperExpr)) {
							la1 = LA(1);
							if (la1 == TT.RBrack)
								goto match6;
							else
								goto stop2;
						} else
							goto stop2;
					}
					break;
				case TT.Id:
				case TT.Number:
				case TT.OtherLit:
				case TT.SQString:
				case TT.String:
				case TT.Symbol:
					{
						if (context.CanParse(P_SuperExpr))
							goto match6;
						else
							goto stop2;
					}
				case TT.At:
					{
						if (context.CanParse(P_SuperExpr)) {
							la1 = LA(1);
							if (la1 == TT.LBrack)
								goto match6;
							else
								goto stop2;
						} else
							goto stop2;
					}
				case TT.PrefixOp:
					{
						if (context.CanParse(P_SuperExpr)) {
							switch (LA(1)) {
							case TT.At:
							case TT.Id:
							case TT.Number:
							case TT.OtherLit:
							case TT.PrefixOp:
							case TT.PreSufOp:
							case TT.SQString:
							case TT.String:
							case TT.Symbol:
								goto match6;
							case TT.Assignment:
							case TT.BQString:
							case TT.Colon:
							case TT.Dot:
							case TT.NormalOp:
							case TT.Not:
								{
									if (contextA != P_SuperExpr)
										goto match6;
									else
										goto stop2;
								}
							case TT.LBrace:
							case TT.LBrack:
							case TT.LParen:
								goto match6;
							default:
								goto stop2;
							}
						} else
							goto stop2;
					}
				case TT.LBrace:
					{
						if (context.CanParse(P_SuperExpr)) {
							la1 = LA(1);
							if (la1 == TT.RBrace)
								goto match6;
							else
								goto stop2;
						} else
							goto stop2;
					}
				default:
					goto stop2;
				}
				continue;
			match1:
				{
					if (!context.CanParse(prec = InfixPrecedenceOf(LT(0)))) goto end;
					var t = MatchAny();
					var rhs = Expr(prec, out primary);
					e = F.Call((Symbol)t.Value, e, rhs, e.Range.StartIndex, rhs.Range.EndIndex - e.Range.StartIndex);;
					e.BaseStyle = NodeStyle.Operator;
					if (!prec.CanParse(P.NullDot)) primary = e;;
				}
				continue;
			match2:
				{
					Skip();
					var rhs = Expr(P.Primary, out primary);
					
							RVList<LNode> args;
							if (rhs.Calls(S.Tuple)) {
								args = new RVList<LNode>(e).AddRange(rhs.Args);
							} else {
								args = new RVList<LNode>(e, rhs);
							}
							e = primary = F.Call(S.Of, args, e.Range.StartIndex, rhs.Range.EndIndex - e.Range.StartIndex);
							e.BaseStyle = NodeStyle.Operator;;
				}
				continue;
			match3:
				{
					var t = MatchAny();
					e = F.Call(ToSuffixOpName((Symbol)t.Value), e, e.Range.StartIndex, t.EndIndex - e.Range.StartIndex);
					e.BaseStyle = NodeStyle.Operator;
					if (t.Type() == TT.PreSufOp) primary = null; // disallow superexpression after suffix (prefix/suffix ambiguity;
				}
				continue;
			match6:
				{
					var rhs = RVList<LNode>.Empty;
					contextA = P_SuperExpr;
					rhs.Add(Expr(P_SuperExpr, out _));
					// Line 0: greedy(Expr)*
					for (;;) {
						switch (LA0) {
						case TT.Id:
						case TT.Number:
						case TT.OtherLit:
						case TT.SQString:
						case TT.String:
						case TT.Symbol:
							rhs.Add(Expr(P_SuperExpr, out _));
							break;
						case TT.At:
							{
								la1 = LA(1);
								if (la1 == TT.LBrack)
									rhs.Add(Expr(P_SuperExpr, out _));
								else
									goto stop;
							}
							break;
						case TT.PrefixOp:
						case TT.PreSufOp:
							{
								switch (LA(1)) {
								case TT.At:
								case TT.Id:
								case TT.Number:
								case TT.OtherLit:
								case TT.PrefixOp:
								case TT.PreSufOp:
								case TT.SQString:
								case TT.String:
								case TT.Symbol:
									rhs.Add(Expr(P_SuperExpr, out _));
									break;
								case TT.Assignment:
								case TT.BQString:
								case TT.Colon:
								case TT.Dot:
								case TT.NormalOp:
								case TT.Not:
									{
										if (contextA != P_SuperExpr)
											rhs.Add(Expr(P_SuperExpr, out _));
										else
											goto stop;
									}
									break;
								case TT.LBrace:
								case TT.LBrack:
								case TT.LParen:
									rhs.Add(Expr(P_SuperExpr, out _));
									break;
								default:
									goto stop;
								}
							}
							break;
						case TT.Assignment:
						case TT.BQString:
						case TT.Colon:
						case TT.Dot:
						case TT.NormalOp:
						case TT.Not:
							{
								if (contextA != P_SuperExpr) {
									switch (LA(1)) {
									case TT.At:
									case TT.Id:
									case TT.Number:
									case TT.OtherLit:
									case TT.PrefixOp:
									case TT.PreSufOp:
									case TT.SQString:
									case TT.String:
									case TT.Symbol:
										rhs.Add(Expr(P_SuperExpr, out _));
										break;
									case TT.Assignment:
									case TT.BQString:
									case TT.Colon:
									case TT.Dot:
									case TT.NormalOp:
									case TT.Not:
										{
											if (contextA != P_SuperExpr)
												rhs.Add(Expr(P_SuperExpr, out _));
											else
												goto stop;
										}
										break;
									case TT.LBrace:
									case TT.LBrack:
									case TT.LParen:
										rhs.Add(Expr(P_SuperExpr, out _));
										break;
									default:
										goto stop;
									}
								} else
									goto stop;
							}
							break;
						case TT.LBrack:
							{
								la1 = LA(1);
								if (la1 == TT.RBrack)
									rhs.Add(Expr(P_SuperExpr, out _));
								else
									goto stop;
							}
							break;
						case TT.LParen:
							{
								la1 = LA(1);
								if (la1 == TT.RParen)
									rhs.Add(Expr(P_SuperExpr, out _));
								else
									goto stop;
							}
							break;
						case TT.LBrace:
							{
								la1 = LA(1);
								if (la1 == TT.RBrace)
									rhs.Add(Expr(P_SuperExpr, out _));
								else
									goto stop;
							}
							break;
						default:
							goto stop;
						}
					}
				stop:;
					e = MakeSuperExpr(e, ref primary, rhs);
				}
			}
		stop2:;
			end: return attrs == null ? e : e.WithAttrs(attrs.ToRVList());
		}
		protected LNode SuperExpr()
		{
			LNode _;
			var e = Expr(StartStmt, out _);
			return e;
		}
		protected LNode SuperExprOpt()
		{
			// Line 0: (SuperExpr | )
			switch (LA0) {
			case TT.Assignment:
			case TT.At:
			case TT.BQString:
			case TT.Colon:
			case TT.Dot:
			case TT.Id:
			case TT.LBrace:
			case TT.LBrack:
			case TT.LParen:
			case TT.NormalOp:
			case TT.Not:
			case TT.Number:
			case TT.OtherLit:
			case TT.PrefixOp:
			case TT.PreSufOp:
			case TT.SQString:
			case TT.String:
			case TT.Symbol:
				{
					var e = SuperExpr();
					return e;
				}
				break;
			default:
				return MissingExpr
				break;
			}
		}
		LNode SuperExprOptUntil(TokenType terminator)
		{
			TT la0, la1;
			LNode e = MissingExpr;
			// Line 0: (SuperExpr)?
			switch (LA0) {
			case TT.Assignment:
			case TT.At:
			case TT.BQString:
			case TT.Colon:
			case TT.Dot:
			case TT.Id:
			case TT.LBrace:
			case TT.LBrack:
			case TT.LParen:
			case TT.NormalOp:
			case TT.Not:
			case TT.Number:
			case TT.OtherLit:
			case TT.PrefixOp:
			case TT.PreSufOp:
			case TT.SQString:
			case TT.String:
			case TT.Symbol:
				e = SuperExpr();
				break;
			}
			bool error = false;
			// Line 0: greedy(&{$LA != terminator} ~(EOF))*
			for (;;) {
				la0 = LA0;
				if (la0 == TT.Semicolon) {
					la0 = LA0;
					if (la0 != terminator) {
						switch (LA(1)) {
						case TT.Semicolon:
							goto match1;
						case TT.At:
						case TT.Id:
						case TT.Number:
						case TT.OtherLit:
						case TT.PrefixOp:
						case TT.PreSufOp:
						case TT.SQString:
						case TT.String:
						case TT.Symbol:
							{
								la1 = LA(1);
								if (la1 != terminator)
									goto match1;
								else
									goto stop;
							}
						case TT.Assignment:
						case TT.BQString:
						case TT.Colon:
						case TT.Dot:
						case TT.NormalOp:
						case TT.Not:
							{
								la1 = LA(1);
								if (la1 != terminator)
									goto match1;
								else
									goto stop;
							}
						case TT.Comma:
						case TT.LBrace:
						case TT.LBrack:
						case TT.LParen:
							{
								la1 = LA(1);
								if (la1 != terminator)
									goto match1;
								else
									goto stop;
							}
						default:
							goto match1;
						}
					} else
						goto stop;
				} else if (!(la0 == EOF || la0 == TT.Semicolon))
					goto match1;
				else
					goto stop;
			match1:
				{
					Check(LA0 != terminator, "$LA != terminator");
					
								if (!error) {
									error = true;
									Error(InputPosition, "Expected " + terminator.ToString());
								}
							;
					Skip();
				}
			}
		stop:;
			return e;
		}
		protected void ExprList(ref RWList<LNode> exprs)
		{
			TT la0;
			exprs = exprs ?? new RWList<LNode>();
			// Line 0: (SuperExpr (TT.Comma SuperExprOpt)* | TT.Comma SuperExprOpt (TT.Comma SuperExprOpt)*)?
			switch (LA0) {
			case TT.Assignment:
			case TT.At:
			case TT.BQString:
			case TT.Colon:
			case TT.Dot:
			case TT.Id:
			case TT.LBrace:
			case TT.LBrack:
			case TT.LParen:
			case TT.NormalOp:
			case TT.Not:
			case TT.Number:
			case TT.OtherLit:
			case TT.PrefixOp:
			case TT.PreSufOp:
			case TT.SQString:
			case TT.String:
			case TT.Symbol:
				{
					exprs.Add(SuperExpr());
					// Line 0: (TT.Comma SuperExprOpt)*
					for (;;) {
						la0 = LA0;
						if (la0 == TT.Comma) {
							Skip();
							exprs.Add(SuperExprOpt());
						} else
							break;
					}
				}
				break;
			case TT.Comma:
				{
					exprs.Add(MissingExpr);
					Skip();
					exprs.Add(SuperExprOpt());
					// Line 0: (TT.Comma SuperExprOpt)*
					for (;;) {
						la0 = LA0;
						if (la0 == TT.Comma) {
							Skip();
							exprs.Add(SuperExprOpt());
						} else
							break;
					}
				}
				break;
			}
		}
		protected void StmtList(ref RWList<LNode> exprs)
		{
			TT la0;
			exprs = exprs ?? new RWList<LNode>();
			var next = SuperExprOptUntil(TT.Semicolon);
			// Line 0: (TT.Semicolon SuperExprOptUntil)*
			for (;;) {
				la0 = LA0;
				if (la0 == TT.Semicolon) {
					exprs.Add(next);
					Skip();
					next = SuperExprOptUntil(TT.Semicolon);
				} else
					break;
			}
			if (next != (object)MissingExpr) exprs.Add(next);;
		}
	}
}