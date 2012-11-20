﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArrowDataBinding.Arrows;

namespace ArrowDataBinding.Combinators
{
    public static partial class Op
    {
        public static InvertibleArrow<A, B>
            Arr<A, B>(this Func<A, B> func, Func<B, A> invFunc)
        {
            /*
             * Basic InvertibleArrow constructor from a Func<A, B> and its inverse Func<B, A>
             */

            return new InvertibleArrow<A, B>(func, invFunc);
        }

        public static InvertibleArrow<A, C>
            Combine<A, B, C>(this InvertibleArrow<A, B> a1, InvertibleArrow<B, C> a2)
        {
            /*
             * Combine two invertible arrows end-to-end
             */

            InvertibleArrow<B, A> a1Reversed = a1.Invert();
            InvertibleArrow<C, B> a2Reversed = a2.Invert();

            InvertibleArrow<A, C> result = new InvertibleArrow<A, C>(
                x => a2.Invoke(a1.Invoke(x)),
                x => a1Reversed.Invoke(a2Reversed.Invoke(x))
                );

            return result;
        }

        public static InvertibleArrow<Tuple<A, C>, Tuple<B, C>>
            First<A, B, C>(this InvertibleArrow<A, B> arr)
        {
            /*
             * Similar to the First function for normal arrows, but obviously performing it on
             * InvertibleArrows instead
             */

            InvertibleArrow<B, A> reversed = arr.Invert();

            return new InvertibleArrow<Tuple<A, C>, Tuple<B, C>>(
                (Tuple<A, C> x) =>
                    new Tuple<B, C>(
                        arr.Invoke(x.Item1),
                        x.Item2
                        ),
                (Tuple<B, C> x) =>
                    new Tuple<A, C>(
                        reversed.Invoke(x.Item1),
                        x.Item2
                        )
                );
        }

        public static InvertibleArrow<Tuple<A, C>, Tuple<B, D>>
            And<A, B, C, D>(this InvertibleArrow<A, B> a1, InvertibleArrow<C, D> a2)
        {
            /*
             * Combines two arrows in parallel leading to an arrow on tuples, similar to the And
             * function on normal arrows
             */

            InvertibleArrow<B, A> a1Inverted = a1.Invert();
            InvertibleArrow<D, C> a2Inverted = a2.Invert();

            // TODO: Make the parallel operator actually parallel for invertible arrows too?
            return new InvertibleArrow<Tuple<A, C>, Tuple<B, D>>(
                (Tuple<A, C> x) =>
                    new Tuple<B, D>(
                        a1.Invoke(x.Item1),
                        a2.Invoke(x.Item2)
                        ),
                (Tuple<B, D> x) =>
                    new Tuple<A, C>(
                        a1.Invert().Invoke(x.Item1),
                        a2.Invert().Invoke(x.Item2)
                        )
                );
        }
    }
}
