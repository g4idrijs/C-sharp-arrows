﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArrowDataBinding.Bindings;
using ArrowDataBinding.Arrows;
using ArrowDataBinding.Combinators;

namespace ArrowDataBinding.demos.project
{
    public class Source : Bindable
    {
        [Bindable]
        public int source { get; set; }

        [Bindable]
        public int multiplies { get; set; }
    }

    public class Destination : Bindable
    {
        [Bindable]
        public int result { get; set; }
    }


    public class BasicBindingDemos
    {
        public void Run()
        {
            Console.WriteLine("Running basic binding demos...");
            RunSimpleDemo();
            RunSimpleInvertibleDemo();
            RunSimpleMultiDemo();
        }

        public static void RunSimpleDemo()
        {
            Source source = new Source();
            Destination dest = new Destination();
            Arrow<int, int> arr = new IDArrow<int>();
            BindingsManager.CreateBinding(source.GetBindPoint("source"), arr, dest.GetBindPoint("result"));

            bool passed = true;
            Random rand = new Random();

            for (int i = 0; i < 100; i++)
            {
                int next = rand.Next();
                source.source = next;
                if (dest.result != source.source) passed = false;
            }

            if (passed) Console.WriteLine("Works!");
            else Console.WriteLine("Doesn't work D:");
        }

        public static void RunSimpleInvertibleDemo()
        {
            Source source = new Source();
            Destination dest = new Destination();
            InvertibleArrow<int, int> arr = Op.Arr((int x) => x + 1, (int x) => x - 1);
            BindingsManager.CreateBinding(source.GetBindPoint("source"), arr, dest.GetBindPoint("result"));

            bool passed = true;
            Random rand = new Random();

            for (int i = 0; i < 100; i++)
            {
                int next = rand.Next();
                source.source = next;
                if (dest.result != next + 1) passed = false;
                dest.result -= 1;
                if (source.source != next - 1) passed = false;
            }

            if (passed) Console.WriteLine("Invertible works too!");
            else Console.WriteLine("Invertible doesn't work tho D:");
        }

        public static void RunSimpleMultiDemo()
        {
            Source source = new Source();
            Destination dest = new Destination();

            Arrow<Tuple<int, int>, int> multiplier = Op.Arr((Tuple<int, int> x) => x.Item1 * x.Item2);

            Arrow<int, int> square = Op.Arr((int x) => x * x);
            Func<int, int, int> add = (int x, int y) => x + y;

            Arrow<Tuple<int, int>, int> pythagoras = Op.And(square, square)
                .Unsplit(add)
                .Combine(Op.Arr((int x) => (int)Math.Sqrt(x)));

            BindingsManager.CreateBinding(BindingsManager.BindPoints(new BindPoint(source, "source"), new BindPoint(source, "multiplies")),
                pythagoras,
                BindingsManager.BindPoints(new BindPoint(dest, "result")));
            source.multiplies = 2;
            source.source = 3;
            Console.WriteLine(dest.result);
        }
    }
}
