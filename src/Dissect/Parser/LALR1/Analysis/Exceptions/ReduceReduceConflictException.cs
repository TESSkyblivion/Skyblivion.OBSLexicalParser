using Dissect.Parser.LALR1.Analysis;
using Dissect.Parser;
using System.Linq;

namespace Dissect.Parser.LALR1.Analysis.Exceptions
{
    /*
     * Thrown when a grammar is not LALR(1) and exhibits
     * a reduce/reduce conflict.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class ReduceReduceConflictException : ConflictException
    {
        /*
        * Returns the first conflicting rule.
         *
         *  The first conflicting rule.
        */
        public Rule FirstRule { get; protected set; }
        /*
        * Returns the second conflicting rule.
         *
         *  The second conflicting rule.
        */
        public Rule SecondRule { get; protected set; }
        /*
        * Returns the conflicting lookahead.
         *
         *  The conflicting lookahead.
        */
        public string Lookahead { get; protected set; }
        /*
        * Constructor.
         *
         *  The number of the inadequate state.
         *  The first conflicting grammar rule.
         *  The second conflicting grammar rule.
         *  The conflicting lookahead.
         *  The faulty automaton.
        */
        public ReduceReduceConflictException(int state, Rule firstRule, Rule secondRule, string lookahead, Automaton automaton)
            : base(GetMessage(state, firstRule, secondRule, lookahead), state, automaton)
        {
            this.FirstRule = firstRule;
            this.SecondRule = secondRule;
            this.Lookahead = lookahead;
        }

        private static string GetMessage(int state, Rule firstRule, Rule secondRule, string lookahead)
        {
            string[] components1 = firstRule.Components;
            string[] components2 = secondRule.Components;
            return
@"The grammar exhibits a reduce/reduce conflict on rules:

  " + firstRule.Number+ @". " + firstRule.Name+ @" -> " + (!components1.Any() ? "/* empty */" : string.Join(" ", components1)) + @"

vs:

  " + secondRule.Number+ @". " + secondRule.Name+ @" -> " + (!components2.Any() ? "/* empty */" : string.Join(" ", components2)) + @"

(on lookahead """ + lookahead + @""" in state " + state + @"). Restructure your grammar or choose a conflict resolution mode.";
        }
    }
}