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
        private List<double[]> _padroesDeEntrada = new List<double[]>();
        private double[,] _padraoEntrada = new double[6,7];
        private double[,] _reconhecimento = new double[6, 7];
        private int _camadaDeEntrada = 6 * 7; // 6 linhas, 7 colunas
        private static int _camdaDeSaida = 4; // Qual jogo deve ser o vencedor - numero de bits para contar até 8
        private DataSet _conjuntoDeTreinamento;
        private Backpropagation _rna;
        private int _azul = 0;
        private int[,] _posicoes;
        private int conjuntoIndex = 0;

        public Form1() {
            InitializeComponent();
            _rna = new Backpropagation(_camadaDeEntrada, _camdaDeSaida, _camadasIntermediarias) {
                ETA = 0.3f,
                Error = 0.005f,
                maxIterationNumber = 5000
            };
            _conjuntoDeTreinamento = new DataSet(_camadaDeEntrada, _camdaDeSaida);
            btnTreinar.Enabled = false;
            btnReconhecer.Enabled = false;
            groupTabuleiroReconhecido.Enabled = false;
        }
        
        //Treina a RNA
        private void btnTreinar_Click(object sender, EventArgs e) {
            _rna.Learn(_conjuntoDeTreinamento);
            btnTreinar.Enabled = false;
            btnReconhecer.Enabled = true;
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

            //É uma peça azul
            if (_azul % 2 == 0) {
                if (btnReconhecer.Enabled) {
                    _reconhecimento[(int) posicao[1], (int) posicao[0]] = 1;
                } else {
                    _padraoEntrada[(int)posicao[1], (int)posicao[0]] = 1;
                }
            } else { //É uma peça vermelha
                if (btnReconhecer.Enabled) {
                    _reconhecimento[(int)posicao[1], (int)posicao[0]] = 2;
                } else {
                    _padraoEntrada[(int)posicao[1], (int)posicao[0]] = 2;
                }
            }
            
            _azul++;
        }

        //Transforma a posição global do checkbox em uma posição no vetor
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
            //Serializa o padrão
            var serializedPattern = SerializePattern(_padraoEntrada);

            _padroesDeEntrada.Add(serializedPattern);

            DataSetObject obj = null;
            
            //Adicona o padrão com sua saída esperada em binário
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
            ResetarTabuleiro();

            UpdateLabelPadroes();

            //Resetar variáveis de controle
            _azul = 0;
            _padraoEntrada = new double[6,7];
        }

        //Reseta os tabuleiros
        private void ResetarTabuleiro() {
            foreach (var control in groupTabuleiro.Controls) {
                if (control.GetType() == typeof(CheckBox)) {
                    ((CheckBox)control).BackColor = Color.Transparent;
                    ((CheckBox)control).Checked = false;
                    ((CheckBox)control).Enabled = true;
                }
            }

            //Se estamos reconhecendo, resetar tabuleiro de reconhecimento e variável de entrada
            if (btnReconhecer.Enabled) {
                _azul = 0;
                _reconhecimento = new double[6, 7];
                foreach (var control in groupTabuleiroReconhecido.Controls) {
                    if (control.GetType() == typeof(CheckBox)) {
                        ((CheckBox)control).BackColor = Color.Transparent;
                        ((CheckBox)control).Checked = false;
                        ((CheckBox)control).Enabled = true;
                    }
                }
            }

        }

        //Transforma um padrão de entrada em um vetor
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

        //Atualiza os labels de informação para o usuário
        private void UpdateLabelPadroes() {
            labelPadroes.Text = $"{_conjuntoDeTreinamento.Length()}/8";
            if (_conjuntoDeTreinamento.Length() == 8) {
                btnTreinar.Enabled = true;
                btnAdicionar.Enabled = false;
                labelStatus.Text = "Reconhecendo";
            }
        }

        //Reconhece o jogo
        private void btnReconhecer_Click(object sender, EventArgs e) {
            var serializedPattern = SerializePattern(_reconhecimento);
            var resultado = _rna.Recognize(serializedPattern);
            var resultadoBinario = "";
            foreach (var b in resultado) {
                if (b >= 0.5f) {
                    resultadoBinario += "1";
                } else {
                    resultadoBinario += "0";
                }
            }

            

            var posPadrao = Convert.ToInt32(resultadoBinario, 2);

            var padraoReconhecido = _padroesDeEntrada.ElementAt(posPadrao);

            Console.WriteLine(padraoReconhecido);
            Console.WriteLine(padraoReconhecido);

            ResetarTabuleiro();
            //Escrever padrao
            var checkboxes = groupTabuleiroReconhecido.Controls.OfType<CheckBox>();
            for (int i = 0; i < padraoReconhecido.Length; i++) {
                Debug.WriteLine(checkboxes.ElementAt(i).Name);
                if (padraoReconhecido[i] == 0) {
                    continue;
                }
                
                checkboxes.ElementAt(i).BackColor = padraoReconhecido[i] == 1 ? Color.Blue : Color.Red;
                checkboxes.ElementAt(i).Checked = true;
            }
        }
    }
}
