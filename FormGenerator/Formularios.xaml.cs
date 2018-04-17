using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Data;

namespace FormGenerator
{
    /// <summary>
    /// Interaction logic for Formularios.xaml
    /// </summary>
    public partial class Formularios : Window
    {
        enum EstadoTela
        {
            INICIAL,
            SELECIONADO,
            NOVO,
            EDITAR
        };

        private EstadoTela estadoTela;
        private int id_formulario;

        public Formularios()
        {
            InitializeComponent();

            estadoTela = EstadoTela.INICIAL;
            MudaEstadoTela(estadoTela);

            LimparCampos();
            LoadFormularios();
        }

        private void LoadFormularios()
        {
            lstFormularios.ItemsSource = Util.LoadFormularios();
        }

        private void LimparCampos()
        {
            txtNomeFormulario.Text = "";
            txtXML.Text = "";
            id_formulario = 0;
        }

        private void lstFormularios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstFormularios.SelectedItem != null)
            {
                DataRowView drv = lstFormularios.SelectedItem as DataRowView;

                txtNomeFormulario.Text = drv.Row["nome_formulario"].ToString();
                txtXML.Text = drv.Row["xml_formulario"].ToString();
                id_formulario = int.Parse(drv.Row["id_formulario"].ToString());

                estadoTela = EstadoTela.SELECIONADO;
                MudaEstadoTela(estadoTela);
            }
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            estadoTela = EstadoTela.NOVO;
            MudaEstadoTela(estadoTela);

            LimparCampos();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(Util.connectionString);

            conn.Open();

            SQLiteCommand comando;
            string strOperacao = "";

            if (estadoTela == EstadoTela.NOVO)
            {
                comando = new SQLiteCommand("INSERT INTO tb_formulario (nome_formulario, xml_formulario) VALUES (?,?)", conn);
                comando.Parameters.Add(new SQLiteParameter("nome_formulario", txtNomeFormulario.Text));
                comando.Parameters.Add(new SQLiteParameter("xml_formulario", txtXML.Text));

                strOperacao = "incluído";
            }
            else
            {
                comando = new SQLiteCommand("UPDATE tb_formulario SET nome_formulario = ?, xml_formulario = ? WHERE id_formulario = ?", conn);
                comando.Parameters.Add(new SQLiteParameter("nome_formulario", txtNomeFormulario.Text));
                comando.Parameters.Add(new SQLiteParameter("xml_formulario", txtXML.Text));
                comando.Parameters.Add(new SQLiteParameter("id_formulario", id_formulario));

                strOperacao = "alterado";
            }

            comando.ExecuteNonQuery();

            MessageBox.Show(String.Format("Formulário {0} com sucesso.", strOperacao));

            conn.Close();

            estadoTela = EstadoTela.INICIAL;
            MudaEstadoTela(estadoTela);

            LimparCampos();
            LoadFormularios();

        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            estadoTela = EstadoTela.EDITAR;
            MudaEstadoTela(estadoTela);
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Confirma a exclusão do formulário?", "Confirmação", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SQLiteConnection conn = new SQLiteConnection(Util.connectionString);

                conn.Open();

                SQLiteCommand comando;
                string strOperacao = "";

                comando = new SQLiteCommand("DELETE FROM tb_formulario WHERE id_formulario = ?", conn);
                comando.Parameters.Add(new SQLiteParameter("id_formulario", id_formulario));

                comando.ExecuteNonQuery();

                MessageBox.Show(String.Format("Formulário excluído com sucesso.", strOperacao));

                lstFormularios.UpdateLayout();

                conn.Close();

                estadoTela = EstadoTela.INICIAL;
                MudaEstadoTela(estadoTela);

                LimparCampos();
                LoadFormularios();
            }

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            estadoTela = EstadoTela.INICIAL;
            MudaEstadoTela(estadoTela);

            LimparCampos();
        }

        private void MudaEstadoTela(EstadoTela estado)
        {
            switch (estado)
            {
                case EstadoTela.INICIAL:
                    txtNomeFormulario.IsEnabled = false;
                    txtXML.IsEnabled = false;

                    btnNovo.IsEnabled = true;
                    btnEditar.IsEnabled = false;
                    btnExcluir.IsEnabled = false;
                    btnCancelar.IsEnabled = false;
                    btnSalvar.IsEnabled = false;

                    lstFormularios.SelectedIndex = -1;

                    break;

                case EstadoTela.SELECIONADO:
                    txtNomeFormulario.IsEnabled = false;
                    txtXML.IsEnabled = false;

                    btnNovo.IsEnabled = false;
                    btnEditar.IsEnabled = true;
                    btnExcluir.IsEnabled = true;
                    btnCancelar.IsEnabled = true;
                    btnSalvar.IsEnabled = true;
                    
                    break;

                case EstadoTela.NOVO:
                    txtNomeFormulario.IsEnabled = true;
                    txtXML.IsEnabled = true;

                    btnNovo.IsEnabled = true;
                    btnEditar.IsEnabled = false;
                    btnExcluir.IsEnabled = false;
                    btnCancelar.IsEnabled = true;
                    btnSalvar.IsEnabled = true;

                    txtNomeFormulario.Focus();

                    break;

                case EstadoTela.EDITAR:
                    txtNomeFormulario.IsEnabled = true;
                    txtXML.IsEnabled = true;

                    btnNovo.IsEnabled = false;
                    btnEditar.IsEnabled = true;
                    btnExcluir.IsEnabled = false;
                    btnCancelar.IsEnabled = true;
                    btnSalvar.IsEnabled = true;

                    txtNomeFormulario.Focus();

                    break;

                default:
                    break;
            }
        }
    }
}
