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
    public partial class Form1 : Form {
        private int[] _camadasIntermediarias = {16};
        private List<double[,]> _padroesDeEntrada = new List<double[,]>();
        private double[,] _padraoEntrada = new double[6,7];
        private int _camadaDeEntrada = 6 * 7; // 6 linhas, 7 colunas
        private static int _camdaDeSaida = 4; // Qual jogo deve ser o vencedor - numero de bits para contar até 8
        private DataSet _conjuntoDeTreinamento;
        private Backpropagation _rna;
        private int _azul = 0;
        private int[,] _posicoes;
        private int conjuntoIndex = 0;

        public Form1() {
            InitializeComponent();
            _rna = new Backpropagation(_camadaDeEntrada, _camdaDeSaida, _camadasIntermediarias);
            _conjuntoDeTreinamento = new DataSet(_camadaDeEntrada, _camdaDeSaida);
            btnTreinar.Enabled = false;
        }
        

        private void btnTreinar_Click(object sender, EventArgs e) {
            _rna.Learn(_conjuntoDeTreinamento);

            //KohonenNeuron[,] camada = ((Kohonen) _rna).GetCompetitiveLayer();
            //Console.WriteLine(camada);
        }

        //Controla a cor dos checkboxes e adiciona-os aos padroes de entrada
        private void checkBoxChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox) sender;

            if (!checkbox.Enabled) {
                return;
            }

            checkbox.Enabled = false;
            checkbox.BackColor = _azul % 2 == 0 ? Color.Blue : Color.Red;

            var posicao = GetPosition(checkbox.Location);
            //var pos = _posicoes[(int)posicao[1], (int)posicao[0]];

            var p = new double[6,7];
            //É uma peça azul
            if (_azul % 2 == 0) {
                /*p[(int)posicao[1], (int)posicao[0]] = 1;
                _padroesDeEntrada.Add(p);*/
                _padraoEntrada[(int) posicao[1], (int) posicao[0]] = 1;
            } else { //É uma peça vermelha
                /*p[(int)posicao[1], (int)posicao[0]] = 2;
                _padroesDeEntrada.Add(p);*/
                _padraoEntrada[(int)posicao[1], (int)posicao[0]] = 1;
            }


            /*
            _padroesDeEntrada.Add(new double[pos, {
                posicao[1],
                posicao[0],
                red % 2 == 0 ? 1 : 0,
                0 }] );*/

            /*var obj = new DataSetObject(new double [] { posicao[1],
                posicao[0],
                red %2 == 0 ? 1 : 0,
                0 });*/

            
            
            /*
            Debug.WriteLine("!ANTES:");
            _conjuntoDeTreinamento.data[pos] = obj;*/

            _azul++;
        }

        private void TreinarRna() {
            
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

        //Adicionar padrão de entrada
        private void button1_Click(object sender, EventArgs e) {
            var serializedPattern = SerializePattern(_padraoEntrada);
            DataSetObject obj = null;
            switch (_conjuntoDeTreinamento.Length()) {
                case 0:
                    obj = new DataSetObject(serializedPattern, new double[] { 0, 0, 0, 0 });
                    break;
                case 1:
                    obj = new DataSetObject(serializedPattern, new double[] { 0, 0, 0, 1 });
                    break;
                case 2:
                    obj = new DataSetObject(serializedPattern, new double[] { 0, 0, 1, 0 });
                    break;
                case 3:
                    obj = new DataSetObject(serializedPattern, new double[] { 0, 0, 1, 1 });
                    break;
                case 4:
                    obj = new DataSetObject(serializedPattern, new double[] { 0, 1, 0, 0 });
                    break;
                case 5:
                    obj = new DataSetObject(serializedPattern, new double[] { 0, 1, 0, 1 });
                    break;
                case 6:
                    obj = new DataSetObject(serializedPattern, new double[] { 0, 1, 1, 0 });
                    break;
                case 7:
                    obj = new DataSetObject(serializedPattern, new double[] { 0, 1, 1, 1 });
                    break;
            }
            _conjuntoDeTreinamento.Add(obj);

            //Resetar o tabuleiro
            foreach (var control in groupTabuleiro.Controls) {
                if (control.GetType() == typeof(CheckBox)) {
                    ((CheckBox) control).BackColor = Color.Transparent;
                    ((CheckBox) control).Checked = false;
                    ((CheckBox) control).Enabled = true;
                }
            }

            UpdateLabelPadroes();

            //Resetar variáveis de controle
            _azul = 0;
            _padraoEntrada = new double[6,7];
        }

        //Transforma cada padrão de entrada em um vetor
        private double[] SerializePattern(double[,] original) {
            var result = new double[6 * 7]; // Vetor de retorno = 4 posições para cada posição no tabuleiro
            var resultIndex = 0; // Controla o index no vetor de retorno
            for (int i = 0; i < 6 ; i++) {
                for (int z = 0; z < 7; z++) {
                    result[resultIndex] = original[i, z];
                    resultIndex++;
                }
            }

            return result;
        }

        private void UpdateLabelPadroes() {
            labelPadroes.Text = $"{_conjuntoDeTreinamento.Length()}/8";
            if (_conjuntoDeTreinamento.Length() == 8) {
                btnTreinar.Enabled = true;
                btnAdicionar.Enabled = false;
                labelStatus.Text = "Reconhecendo";
            }
        }
    }
}
