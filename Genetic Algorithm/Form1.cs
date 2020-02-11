using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genetic_Algorithm
{
    public partial class Form1 : Form
    {
        double mutationRate = 0.5;
        double crossover = 0.8;
        public Form1()
        {
            InitializeComponent();
        }
        Random rd = new Random();
        List<rec> All_rec = new List<rec>()
        {
            new rec() { Name ="A" , Weight = 30 },
            new rec() { Name ="B" , Weight =18,Value =75 },
            new rec() { Name = "C" ,Value =45 ,Weight =8},
            new rec() { Name = "D", Value =20 , Weight =5},
            new rec() { Name = "E" , Value =2 ,Weight =1},
            new rec() { Name = "F" ,Value =300,Weight = 50},
            new rec() { Name ="G" , Value = 70, Weight =16},
            new rec() { Name = "H" , Value =50,Weight = 10},
            new rec() { Name = "I",Value = 25 ,Weight =4},
            new rec() { Name ="J" ,Value =3 ,Weight =1}
        };
        //List<rec> All_rec = new List<rec>()
        //{
        //    new rec() { Name ="A" , Weight = 1,Value =20 },
        //    new rec() { Name ="B" , Weight =2,Value =100 },
        //    new rec() { Name = "C" ,Weight =3 ,Value =250},
        //    new rec() { Name = "D", Weight =4 , Value =350},
        //    new rec() { Name = "E" , Weight =5 ,Value =499},
        //    new rec() { Name = "F" ,Weight =6,Value = 550},
        //};
        List<Chromosome> Population = new List<Chromosome>();
        int Max_Weight = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                string[] s = textBox1.Text.Split(',');
                All_rec[0].Value = int.Parse(s[1]);
                Max_Weight = int.Parse(s[0]);
                //Max_Weight = 9;
                for (int i = 2; i < All_rec.Count + 2; i++)
                {
                    All_rec[i - 2].count = int.Parse(s[i]);
                }
                int Max = All_rec.ToList().Max(x => x.count);
               // int Max = 5;
                for (int i = 0; i < 8000; i++)
                {
                    Chromosome ch = new Chromosome();
                    ch.value = Randvalue(Max);
                    ch.fitness = calcFitness(ch.value);
                    Population.Add(ch);
                }
                double Max_ans = 0.0;
                double Last_ans = -1000000;
                for (int ii = 0; ii < 100; ii++)
                {
                    //if (ii > 100) ii--;
                    List<Chromosome> new_pop = new List<Chromosome>();
                    for (int i = 0; i < Population.Count; i++)
                    {
                        if (rd.NextDouble() > crossover)
                        {
                            new_pop.Add(Population[selection(Population.Count)]);
                        }
                       else
                        {
                            Chromosome parent1 = Population[selection(Population.Count)];
                            Chromosome parent2 = Population[selection(Population.Count)];
                            Chromosome child = parent1.crossover(parent2);
                            double rd_mu = rd.NextDouble();
                            if (rd_mu < mutationRate) child.mutate(Max,All_rec);
                            child.fitness = calcFitness(child.value);
                            new_pop.Add(child);
                        }
                    }
                    Population = new List<Chromosome>();
                    new_pop = new_pop.OrderBy(x => x.fitness).ToList();
                    double sumf = 0.0;

                    foreach (var a in new_pop) sumf += a.fitness;
                    sumf = Math.Abs(sumf);
                    int Allpop = new_pop.Count;
                    int nowcount = Allpop;
                    for (int j = new_pop.Count - 1; j >= 0; j--)
                    {
                        int count = (int)((new_pop[j].fitness / sumf) * Allpop + 0.5);
                        if (count > nowcount) count = nowcount;
                        else if (count < 0) count = 0;
                        for (int z = 0; z < count; z++)
                        {
                            Population.Add(new_pop[j]);
                        }
                        nowcount -= count;
                        if (nowcount == 0)
                        {
                            break;
                        }
                    }
                    while (nowcount != 0)
                    {
                        int rdn1 = rd.Next(0, new_pop.Count);
                        int rdn2 = rd.Next(0, new_pop.Count);
                        if (new_pop[rdn1].fitness > new_pop[rdn2].fitness) Population.Add(new_pop[rdn1]);
                        else Population.Add(new_pop[rdn2]);
                        nowcount--;
                    }
                    Population = Population.OrderBy(x => x.fitness).ToList();
                    Max_ans = Population[Population.Count - 1].fitness;
                    if (Max_ans > Last_ans)
                    {
                        if (Last_ans < 0 && Max_ans > 0) textBox2.Text += "-------------------------------------------------------------------" + "\r\n";
                        Last_ans = Max_ans;
                        textBox2.Text += ("Genetic Times：" + ii.ToString()).ToString().PadRight(("Genetic Times：500000000").ToString().Length- ("Genetic Times：" + ii.ToString()).ToString().Length,' ') + ("  |  fitness：" + Max_ans ).ToString().PadRight(("  |  fitness：" + "-100000000").Length- ("  |  fitness：" + Max_ans).ToString().Length,' ') + "      "+System.DateTime.Now.Hour.ToString("00")+"："+System.DateTime.Now.Minute.ToString("00")+"："+System.DateTime.Now.Second.ToString("00") +"\r\n";
                    }
                    GC.Collect();
                }
                double ans = 0;
                for (int i = 0; i < All_rec.Count; i++)
                {
                    ans += Population[Population.Count - 1].value[i] * All_rec[i].Value;
                }
                MessageBox.Show(Last_ans.ToString());
            });
        }

        public int selection(int count)
        {
            //eturn (int)Math.Floor(Math.Sqrt(rd.Next(((count * count) / 2))));
            return rd.Next(count);
        }

        public int[] Randvalue(int Maxcount)
        {
            int[] value = new int[10];
            for(int i = 0; i < 10 ; i++)
            {
                value[i] = rd.Next(0, All_rec[i].count+1);
            }
            return value;
        }
        public double calcFitness(int[] value)
        {
            double fitness = 0.0;
            int weight = 0;
            for(int i = 0; i < All_rec.Count; i++)
            {
                if (value[i] > All_rec[i].count || ((weight + value[i] * All_rec[i].Weight) > Max_Weight))
                {
                    fitness += value[i] *- 100;
                }
                else
                {
                    fitness += value[i] * All_rec[i].Value;
                    weight += (int)value[i] * (int)All_rec[i].Weight;
                }
            }
            return fitness;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }
    }
    public class rec
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public double Value { get; set; }
        public int count { get; set; }
        public rec Convert()
        {
            rec r = new rec();
            r.count = count;
            r.Name = Name;
            r.Weight = Weight;
            r.Value = Value;
            return r;
        }
    }
    class Chromosome
    {
        Random rd = new Random();
        public double fitness=0.0;
        public int[] value =new  int[10];
        public  double calcFitness()
        {
            return 0.0;
        }
        public  Chromosome crossover(Chromosome parent)
        {
           Chromosome child = new Chromosome();
           int rdn = rd.Next(this.value.Length);
           for(int i = 0; i < rdn; i++)
               child.value[i] = this.value[i];
           for(int i = rdn; i < this.value.Length; i++)
               child.value[i] = parent.value[i];
            return child;
        } 
        public  void mutate(int Max,List<rec> rec)
        {
            int rdn = rd.Next(0,value.Length);
            value[rdn] = rd.Next(0,rec[rdn].count+1);
        }
    }
}

