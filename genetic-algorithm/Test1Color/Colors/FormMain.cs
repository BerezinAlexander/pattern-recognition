using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Colors
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        // Эталонный цвет к которому идет приближение
        Color goal;
        // Количество особей
        const int N = 10;
        // Сами особи
        List<Specimen> specimens = new List<Specimen>();
        List<Specimen> condidates = new List<Specimen>();
        // Количетсво итераций
        const int STEPS = 3000;
        // Для функции подсчета веротяности скрещивания
        const int DIVERSITY_COEFFICIENT = 10;
        // График
        Pen pen = new Pen(Color.FromArgb(0, 0, 0));
        Point ptFrom = new Point();
        Point ptTo = new Point();
        Graphics gr;

        private bool FindColor()
        {
            // Создание начальной популяции
            InitSpecimens(N);
            ShowSpecimens(specimens);
            TeachSpecimens(specimens);
            return false;
        }

        private void InitSpecimens(int count)
        {
            for (int i = 0; i < count; i++)
            {
                specimens.Add(new Specimen(count));
                condidates.Add(new Specimen(count));
            }
        }

        private void ShowSpecimens(List<Specimen> spec)
        {
            for (int i = 0; i < spec.Count; i++)
            {
                logText.AppendText(spec[i].ToString(i));
            }
        }

        private void TeachSpecimens(List<Specimen> spec)
        {
            Random random = new Random();
            CopySpecimens(condidates, spec);
            // Итерации обучения
            for (int step = 0; step < STEPS; ++step)
            {
                // Отбор
                double sumProbability;   // Сумма вероятностей для определения нынешнего номера особи
                int godFinger;  // Выбор особи
                int currentSelection = 0;   // Номер особи

                for (int j = 0; j < N; ++j)
                {
                    sumProbability = 0;
                    godFinger = random.Next(100);
                    for (int i = 0; i < N; ++i)
                    {
                        sumProbability += spec[i].GetCP();
                        if ((sumProbability * 100) > godFinger)
                        {
                            currentSelection = i;
                            break;
                        }
                    }
                    condidates[j].CopySpecimen(spec[currentSelection]);
                }

                if ((step + 1) % N == 0)
                {
                    logText.AppendText("Pre calc:\n");
                    logText.AppendText("Step:" + (step + 1).ToString() + "\n");
                    logText.AppendText("Specimens:\n");
                    ShowSpecimens(spec);
                    logText.AppendText("Condidates:\n");
                    ShowSpecimens(condidates);
                }

                //CrossoverAndMutateSpecimens(specimens);
                //SetFitness(specimens);
                //CalcProbability(specimens);

                //DrawGraph(specimens, step);
                CrossoverAndMutateSpecimens(condidates);
                SetFitness(condidates);
                CalcProbability(condidates);

                DrawGraph(condidates, step);

                if ((step + 1) % N == 0)  //???
                {
                    logText.AppendText("Step:" + (step + 1).ToString() + "\n");
                    logText.AppendText("Specimens:\n");
                    ShowSpecimens(spec);
                    logText.AppendText("Condidates:\n");
                    ShowSpecimens(condidates);
                }
            }
            int speciNumber = TestSpecimens(condidates);
            if (speciNumber != -1)
            {
                pFoundOne.BackColor = spec[speciNumber].GetColor();
            }
            CopySpecimens(spec, condidates);
        }

        private void CopySpecimens(List<Specimen> to, List<Specimen> from)
        {
            for (int i = 0; i < from.Count; i++)
            {
                to[i].SetColor(from[i].GetColor());
                to[i].SetCP(from[i].GetCP());
            }
        }

        private int Fitness(Color goal, Specimen candidate)
        {
            int result =
                Math.Abs(goal.R - candidate.GetRed());
                //+ Math.Abs(goal.G - candidate.GetGreen())
                //+ Math.Abs(goal.B - candidate.GetBlue());
            return result;
        }

        private int StayInByte(int num)
        {
            if (num < byte.MinValue)
                return byte.MinValue;
            else if (num > byte.MaxValue)
                return byte.MaxValue;
            else
                return num;
        }

        private void SetFitness(List<Specimen> specimens)
        {
            for (int i = 0; i < specimens.Count; i++)
            {
                specimens[i].SetFit(Fitness(goal, specimens[i]));
            }
        }

        private void DrawGraph(List<Specimen> specimens, int currentStep)
        {
            double fitSum = 0;
            foreach (Specimen specimen in specimens)
            {
                fitSum += specimen.GetFit();
            }
            fitSum /= specimens.Count;
            ptTo = new Point(Convert.ToInt32(currentStep * pGraph.Width / STEPS), Convert.ToInt32(fitSum / N * pGraph.Height / (byte.MaxValue * 3)));
            if (currentStep == 0)
                ptFrom = ptTo;
            gr.DrawLine(pen, ptFrom, ptTo);
            ptFrom = ptTo;
        }

        

        // Скрещивание ( функция ГОВНО !!! )
        private void CrossoverAndMutateSpecimens(List<Specimen> pretenders)
        {
            Random random;
            int godFinger;
            
            //int R1, R2, G1, G2, B1, B2;
            for (int i = 0; i < N - 2 /*N / 2*/; ++i)
            {
                random = new Random();
                //// Скрещивание
                //godFinger = random.Next(100);
                //R1 = pretenders[i + (godFinger < 50 ? 0 : 1)].GetRed();
                //R2 = pretenders[i + (godFinger < 50 ? 1 : 0)].GetRed();
                //godFinger = random.Next(100);
                //G1 = pretenders[i + (godFinger < 50 ? 0 : 1)].GetGreen();
                //G2 = pretenders[i + (godFinger < 50 ? 1 : 0)].GetGreen();
                //godFinger = random.Next(100);
                //B1 = pretenders[i + (godFinger < 50 ? 0 : 1)].GetBlue();
                //B2 = pretenders[i + (godFinger < 50 ? 1 : 0)].GetBlue();
                // Скрещивание временно просто как присваивание цвета
                godFinger = random.Next(100);
                if (godFinger < 50)
                {
                    int col = pretenders[i].GetBlue();
                    int col2 = pretenders[i+1].GetBlue();

                    int tmp = col & 240; // 11110000
                    int tmp2 = col2 & 31; // 00001111
                    int res = tmp | tmp2;
                    pretenders[i].SetColor(Color.FromArgb(res, res, res));

                    tmp = col2 & 240; // 11110000
                    tmp2 = col & 31; // 00001111
                    res = tmp | tmp2;
                    pretenders[i+1].SetColor(Color.FromArgb(res, res, res));
                                      
                    ++i;
                }
                    
                //else
                //    pretenders[i + 1].SetColor(pretenders[i].GetColor());
                
                //// Мутация
                //random = new Random();
                //godFinger = random.Next(300);
                //if (godFinger < 5)
                //    R1 = Mutate(R1, godFinger);
                //else if (godFinger < 10)
                //    R2 = Mutate(R2, godFinger);
                //else if (godFinger < 15)
                //    G1 = Mutate(G1, godFinger);
                //else if (godFinger < 20)
                //    G2 = Mutate(G2, godFinger);
                //else if (godFinger < 25)
                //    B1 = Mutate(B1, godFinger);
                //else if (godFinger < 30)
                //    B2 = Mutate(B2, godFinger);
                // Мутация
                Mutate(pretenders[i]);
                Mutate(pretenders[i + 1]);
            }
        }

        //// Мутация
        //private int Mutate(int from, int mutation)
        //{
        //    mutation = (mutation % 5) - 2;
        //    return StayInByte(from + mutation);
        //}
        // Мутация
        private void Mutate(Specimen speci)
        {
            Random rnd = new Random();
            int mutation = rnd.Next(100);
            if (mutation < 10)
                mutation = (mutation % 5) - 2;
            else
                return;
            int from = speci.GetBlue();
            mutation = StayInByte(from + mutation);
            speci.SetColor(Color.FromArgb(mutation, mutation, mutation));
        }

        // Приспособленность
        private int TestSpecimens(List<Specimen> specimens)
        {
            for (int i = 0; i < specimens.Count; i++)
            {
                specimens[i].SetFit(Fitness(goal, specimens[i]));
                if (specimens[i].GetFit() == 0)
                    return i;
            }
            return -1;
        }

        // Вычисление выроятности скрещивания
        private void CalcProbability(List<Specimen> specimens)
        {
            //double sumFitness = 0;
            //double sumProbability = 0;
            //for (int i = 0; i < specimens.Count; i++)
            //{
            //    sumFitness += specimens[i].GetFit();
            //}
            //for (int i = 0; i < specimens.Count; i++)
            //{
            //    specimens[i].SetCP(Math.Exp(DIVERSITY_COEFFICIENT * specimens[i].GetFit() / sumFitness));
            //    sumProbability += specimens[i].GetCP();
            //}
            //for (int i = 0; i < specimens.Count; i++)
            //{
            //    specimens[i].SetCP(specimens[i].GetCP() / sumProbability);
            //}

            double max = 0;
            if (goal.B > 128)
                max = goal.B;
            else
                max = 255d - goal.B;

            double sum = 0;

            for (int i = 0; i < specimens.Count; i++)
            {
                specimens[i].SetCP(1 - (specimens[i].GetFit() / max));
                sum += specimens[i].GetCP();
            }
            //for (int i = 0; i < specimens.Count; i++)
            //{
            //    specimens[i].SetCP(Math.Exp(/*DIVERSITY_COEFFICIENT **/ 1 + specimens[i].GetCP()));
            //    sum2 += specimens[i].GetCP();
            //}
            for (int i = 0; i < specimens.Count; i++)
            {
                specimens[i].SetCP(specimens[i].GetCP() / sum);
            }
        }



        private void ClearAll()
        {
            // Очистка предыдущей попытки
            specimens.Clear();
            condidates.Clear();
            logText.Clear();
            pFoundOne.BackColor = Color.FromKnownColor(KnownColor.Control);
            pGraph.BackColor = Color.FromKnownColor(KnownColor.Control);
            lbColorSelection.Visible = true;
            gr = pGraph.CreateGraphics();   // Куда это правильнее помещать? В OnCreate панели?
            gr.Clear(Color.FromKnownColor(KnownColor.Control));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearAll();

            // Выбор цвета
            colorDialog.ShowDialog();
            goal = colorDialog.Color;
            pChosenOne.BackColor = goal;
            lbColorSelection.Visible = false;

            FindColor();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            for (int i = 0; i < 100; ++i)
            {
                logText.AppendText(i.ToString() + ":" + rnd.Next(10).ToString() + "\n");
            }
        }

        private void btFind_Click(object sender, EventArgs e)
        {

        }
    }
}
