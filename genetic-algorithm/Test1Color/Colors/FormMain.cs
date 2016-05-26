﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        // Количетсво итераций
        const int STEPS = 1000;

        private int Fitness(Color goal, Specimen candidate)
        {
            int result = 
                Math.Abs(goal.R - candidate.getRed())
                + Math.Abs(goal.G - candidate.getGreen())
                + Math.Abs(goal.B - candidate.getBlue());
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

        private void InitSpecimens(int count, List<Specimen> specimens)
        {
            for (int i = 0; i < count; i++)
            {
                specimens.Add(new Specimen(count));
            }
        }

        private void ShowSpecimens(List<Specimen> specimens)
        {
            for (int i = 0; i < specimens.Count; i++)
            {
                logText.AppendText(specimens[i].ToString(i));
            }
        }

        private bool FindColor()
        {
            InitSpecimens(N, specimens);
            ShowSpecimens(specimens);
            for (int step = 0; step < STEPS; step++)
            {

            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Инициализация
            Random rnd = new Random();
            label1.Visible = false;
            Color[] condidates = new Color[N];
            Color[] condidatesPretenders = new Color[N];
            double[] condidatesProbability = new double[N];
            Pen pen = new Pen(Color.FromArgb(0, 0, 0));
            Point ptFrom = new Point();
            Point ptTo = new Point();
            // Выбор цвета
            colorDialog1.ShowDialog();
            Color wantColor = colorDialog1.Color;
            panel1.BackColor = wantColor;

            // Создание начальной популяции
            for (int i = 0; i < N; ++i)
            {
                Color ret = new Color();
                ret = Color.FromArgb(
                    rnd.Next(120, 135),
                    rnd.Next(120, 135),
                    rnd.Next(120, 135));
                condidates[i] = ret;
                condidatesProbability[i] = 1f / N;
                logText.AppendText(condidates[i].ToString() + "\n");
            }

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
                    godFinger = rnd.Next(100);
                    for (int i = 0; i < N; ++i)
                    {
                        sumProbability += condidatesProbability[i];
                        if ((sumProbability * 100) > godFinger)
                        {
                            currentSelection = i;
                            break;
                        }
                    }
                    condidatesPretenders[j] = condidates[currentSelection];
                }

                // Скрещивание
                bool exchanged;   // Флаг указывающий на то, что особи хоть раз поменялись компонентами
                //Color newborn;
                int R1, R2, G1, G2, B1, B2;
                for (int i = 0; i < N / 2; ++i)
                {
                    exchanged = false;
                    godFinger = rnd.Next(100) + 1;
                    if (godFinger < 50)
                    {
                        R1 = condidatesPretenders[i].R;
                        R2 = condidatesPretenders[i + 1].R;
                    }
                    else
                    {
                        R2 = condidatesPretenders[i].R;
                        R1 = condidatesPretenders[i + 1].R;
                        exchanged = true;
                    }

                    godFinger = rnd.Next(100) + 1;
                    if (godFinger < 50)
                    {
                        G1 = condidatesPretenders[i].G;
                        G2 = condidatesPretenders[i + 1].G;
                    }
                    else
                    {
                        G2 = condidatesPretenders[i].G;
                        G1 = condidatesPretenders[i + 1].G;
                        exchanged = true;
                    }

                    if (!exchanged)
                    {
                        B2 = condidatesPretenders[i].B;
                        B1 = condidatesPretenders[i + 1].B;
                    }
                    else
                    {
                        godFinger = rnd.Next(100) + 1;
                        if (godFinger < 50)
                        {
                            B1 = condidatesPretenders[i].B;
                            B2 = condidatesPretenders[i + 1].B;
                        }
                        else
                        {
                            B2 = condidatesPretenders[i].B;
                            B1 = condidatesPretenders[i + 1].B;
                        }
                    }

                    // Мутация
                    if (rnd.Next(5) == 0)
                    {
                        int localRnd = rnd.Next(3);
                        switch (localRnd)
                        {
                            case 0:
                                R1 += rnd.Next(10) - 5;
                                R1 = stayInChar(R1);
                                break;
                            case 1:
                                G1 += rnd.Next(10) - 5;
                                G1 = stayInChar(G1);
                                break;
                            case 5:
                                B1 += rnd.Next(10) - 5;
                                B1 = stayInChar(B1);
                                break;
                        }
                    }
                    if (rnd.Next(5) == 0)
                    {
                        int localRnd = rnd.Next(3);
                        switch (localRnd)
                        {
                            case 0:
                                R2 += rnd.Next(10) - 5;
                                R2 = stayInChar(R2);
                                break;
                            case 1:
                                G2 += rnd.Next(10) - 5;
                                G2 = stayInChar(G2);
                                break;
                            case 5:
                                B2 += rnd.Next(10) - 5;
                                B2 = stayInChar(B2);
                                break;
                        }
                    }

                    // Заполнение массива с особями
                    condidates[i] = Color.FromArgb(R1, G1, B1);

                    condidates[i + 1] = Color.FromArgb(R2, G2, B2);
                }

                int fitSumOne = 0;
                double fitSumTwo = 0;
                int minFit = int.MaxValue;
                // Проверка на приспособленность
                for (int i = 0; i < N; ++i)
                {
                    int fitCoeff = fitness(wantColor, condidates[i]);
                    if (fitCoeff == 0)
                    {
                        panel2.BackColor = condidates[i];
                        return;
                    }
                    else
                    {
                        if (minFit > fitCoeff)
                            minFit = fitCoeff;
                        condidatesProbability[i] = fitCoeff;
                        fitSumOne += fitCoeff;
                    }
                }

                // Рисование графика приближения к эталону
                Graphics gr = panGraph.CreateGraphics();
                ptTo = new Point(Convert.ToInt32(step * panGraph.Width / STEPS), Convert.ToInt32(fitSumOne / N * panGraph.Height / 765));
                gr.DrawLine(pen, ptFrom, ptTo);
                ptFrom = ptTo;

                fitSumOne -= minFit * 10;
                // Расстановка вероятностей
                for (int i = 0; i < N; ++i)
                {
                    condidatesProbability[i] -= minFit - 1;
                    //condidatesProbability[i] = 1 - (condidatesProbability[i] / fitSumOne);
                    //condidatesProbability[i] = 1 / (1 - Math.Exp(-1 * condidatesProbability[i]));
                    condidatesProbability[i] = Math.Exp(condidatesProbability[i]);
                    fitSumTwo += condidatesProbability[i];
                }
                for (int i = 0; i < N; ++i)
                {
                    condidatesProbability[i] = condidatesProbability[i] / fitSumTwo;
                    logText.AppendText(step.ToString() + "/" + i.ToString() + ":" + condidatesProbability[i] + "\n");
                }
                logText.AppendText(step.ToString() + ":" + condidates[0] + "\n");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            for (int i = 0; i < 100; ++i)
            {
                logText.AppendText(i.ToString() + ":" + rnd.Next(10).ToString() + "\n");
            }
            
        }
    }
}
