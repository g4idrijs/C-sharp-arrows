﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArrowDataBinding.Arrows
{
    public abstract class IArrow
    {
        protected Type _a;
        public Type a
        {
            get
            {
                return this._a;
            }
        }

        protected Type _b;
        public Type b
        {
            get
            {
                return this._b;
            }
        }

        public abstract dynamic Invoke<T>(T var);
    }

    public class Arrow<A, B> : IArrow
    {
        private Func<A, B> func;

        public Arrow(Func<A, B> expr)
        {
            this.func = expr;
            this._a = typeof(A);
            this._b = typeof(B);
        }


        public B Invoke(A input)
        {
            return func(input);
        }

        public override dynamic Invoke<T>(T input)
        {
            if (typeof(T) == typeof(A))
            {
                return func((A)Convert.ChangeType(input, typeof(A)));
            }
            else
            {
                throw new Exception("Invalid type supplied to 'Arrow.Invoke'!");
            }
        }


        public Arrow<A, C> Combine<C>(Arrow<B, C> other)
        {
            Func<B, C> otherFunc = other.func;
            Arrow<A, C> result = new Arrow<A, C>(
                x => other.func(this.func(x))
                );

            return result;
        }

        public static void blah()
        {
            Arrow<int, string> test = Op.Arr((int x) => x + 1).Combine(Op.Arr((int x) => x.ToString()));
        }
    }


    public class Op
    {
        public static Arrow<A, B> Arr<A, B>(Func<A, B> func)
        {
            return new Arrow<A, B>(func);
        }

        public static Arrow<A, C> Combine<A, B, C>(Arrow<A, B> a1, Arrow<B, C> a2)
        {
            return a1.Combine<C>(a2);
        }

        public static Arrow<Tuple<A, C>, Tuple<B, C>> First<A, B, C>(Arrow<A, B> arr)
        {
            /*
             * Takes an Arrow<A, B> and returns an arrow from a Tuple<A, C> to a Tuple<B, C> by
             * calling the original arrow function on the first element of the tuple.
             */

            return new Arrow<Tuple<A, C>, Tuple<B, C>>(
                (Tuple<A, C> x) =>
                    new Tuple<B, C>(
                        arr.Invoke(x.Item1),
                        x.Item2
                        )
                );
        }

        public static Arrow<Tuple<A, B>, Tuple<B, A>> Swap<A, B>()
        {
            /*
             * Returns an arrow which takes as input a Tuple<A, B> and returns a Tuple<B, A> which
             * is the input with its values swapped.
             * Used by Second.
             */

            return new Arrow<Tuple<A, B>, Tuple<B, A>>(
                (Tuple<A, B> x) =>
                    new Tuple<B, A> (
                        x.Item2,
                        x.Item1
                        )
                );
        }

        public static Arrow<Tuple<C, A>, Tuple<C, B>> Second<A, B, C>(Arrow<A, B> arr)
        {
            /*
             * Built upon First, but uses the Swap function above in combination with it to return
             * an arrow which runs the input arrow's function on the second argument of the tuple.
             */

            Arrow<Tuple<A, C>, Tuple<B, C>> fstArrow = First<A, B, C>(arr);

            return Swap<C, A>()  // Swap the tuple
                .Combine(fstArrow)  // Run the function on the first element
                .Combine(Swap<B, C>());  // Swap the tuple back
        }

        public static Arrow<Tuple<A, C>, Tuple<B, D>> And<A, B, C, D>(Arrow<A, B> a1, Arrow<C, D> a2)
        {
            return new Arrow<Tuple<A, C>, Tuple<B, D>>(
                (Tuple<A, C> x) =>
                    new Tuple<B, D>(
                        a1.Invoke(x.Item1),
                        a2.Invoke(x.Item2)
                        )
               );
        }
    }
}