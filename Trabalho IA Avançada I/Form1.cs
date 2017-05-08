using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADReNA_API;
using ADReNA_API.Data;
using ADReNA_API.NeuralNetwork;
using DataSet = ADReNA_API.Data.DataSet;

namespace Trabalho_IA_Avançada_I {
    public partial class Form1 : Form  {

        private DataSet _conjuntoDeTreinamento;
        private INeuralNetwork _rna;
        private int red = 0;
        private int[,] _posicoes;

        public Form1() {
            InitializeComponent();
            _conjuntoDeTreinamento = new DataSet(4);
            _posicoes = new int[6,7];
            var pos = 0;
            for (int l = 0; l < 6; l++) {
                for (int c = 0; c < 7; c++) {
                    _conjuntoDeTreinamento.Add(new DataSetObject(new double[] {l,c,1,1}));
                    Debug.WriteLine($"(Linha: {l}, Coluna: {c}");
                    _posicoes[l, c] = pos;
                    pos ++;
                }
            }

            _rna = new Kohonen(4,10,300);
        }
        

        private void btnTreinar_Click(object sender, EventArgs e) {
            _rna.Learn(_conjuntoDeTreinamento);

            KohonenNeuron[,] camada = ((Kohonen) _rna).GetCompetitiveLayer();
            Console.WriteLine(camada);
        }

        private void checkBoxChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox) sender;

            if (!checkbox.Enabled) {
                return;
            }

            checkbox.Enabled = false;
            checkbox.BackColor = red%2 == 0 ? Color.Blue : Color.Red;
            var posicao = GetPosition(checkbox.Location);
            var obj = new DataSetObject(new double [] { posicao[1],
                posicao[0],
                red %2 == 0 ? 1 : 0,
                0 });

            var pos = _posicoes[(int) posicao[1], (int) posicao[0]];


            Debug.WriteLine("!ANTES:");
            _conjuntoDeTreinamento.data[pos] = obj;

            red++;
        }

        private double[] GetPosition(Point location) {
            double[] position = new double[2];
            switch (location.X) {
                case 16:
                    position[0] = 0;
                    break;
                case 37:
                    position[0] = 1;
                    break;
                case 58:
                    position[0] = 2;
                    break;
                case 79:
                    position[0] = 3;
                    break;
                case 100:
                    position[0] = 4;
                    break;
                case 121:
                    position[0] = 5;
                    break;
                case 142:
                    position[0] = 6;
                    break;
            }

            switch (location.Y) {
                case 30:
                    position[1] = 0;
                    break;
                case 50:
                    position[1] = 1;
                    break;
                case 69:
                    position[1] = 2;
                    break;
                case 89:
                    position[1] = 3;
                    break;
                case 109:
                    position[1] = 4;
                    break;
                case 129:
                    position[1] = 5;
                    break;
            }

            return position;
        }
    }
}
