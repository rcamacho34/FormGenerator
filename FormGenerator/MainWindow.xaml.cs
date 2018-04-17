using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace FormGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //txtXML.Text = LoadDefaultXML();
            LoadFormularios();
        }

        private void btnProcessar_Click(object sender, RoutedEventArgs e)
        {
            if (txtXML.Text.Trim() == "")
            {
                MessageBox.Show("Informe o XML a ser processado.");
                return;
            }

            if (pnlForm.Children.Count > 0)
            {
                pnlForm.Children.RemoveRange(0, pnlForm.Children.Count);
            }

            try
            {

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(txtXML.Text);

                XmlNodeList xmlScreen;
                xmlScreen = doc.GetElementsByTagName("Screen");

                XmlNodeList xmlRows;
                xmlRows = xmlScreen[0].ChildNodes;

                XmlNodeList xmlColumn;

                for (int i = 0; i < xmlRows.Count; i++)
                {
                    xmlColumn = xmlRows[i].ChildNodes;

                    XmlNode xmlControle = xmlColumn[0].FirstChild;
                    XmlAttributeCollection atributos = xmlControle.Attributes;

                    switch (xmlControle.Name)
                    {
                        case "Textbox":
                            Label labelTextbox = new Label();
                            labelTextbox.Content = atributos.GetNamedItem("label").Value;
                            pnlForm.Children.Add(labelTextbox);

                            TextBox text = new TextBox();
                            text.Name = atributos.GetNamedItem("name").Value;
                            pnlForm.Children.Add(text);
                            break;

                        case "Numericbox":
                            Label labelNumericbox = new Label();
                            labelNumericbox.Content = atributos.GetNamedItem("label").Value;
                            pnlForm.Children.Add(labelNumericbox);

                            TextBox numero = new TextBox();
                            numero.Name = atributos.GetNamedItem("name").Value;

                            numero.PreviewTextInput += new TextCompositionEventHandler(number_click);

                            pnlForm.Children.Add(numero);
                            break;

                        case "LabelElement":
                            Label label = new Label();
                            label.Name = atributos.GetNamedItem("name").Value;
                            label.Content = atributos.GetNamedItem("label").Value;

                            string type = atributos.GetNamedItem("type").Value;

                            if (type == "small")
                                label.FontSize = 10;
                            else if (type == "medium")
                                label.FontSize = 12;
                            else if (type == "big")
                                label.FontSize = 14;

                            pnlForm.Children.Add(label);
                            break;

                        case "Button":
                            Button button = new Button();
                            button.Name = atributos.GetNamedItem("name").Value;
                            button.Content = " " + atributos.GetNamedItem("label").Value + " ";
                            button.HorizontalAlignment = HorizontalAlignment.Left;
                            pnlForm.Children.Add(button);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (XmlException ex)
            {
                MessageBox.Show("O XML informado é inválido.");
            }
            catch (Exception ex2)
            {
                MessageBox.Show("Erro: " + ex2.Message);
            }
        }

        private void number_click(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private string LoadDefaultXML()
        {
            return "<?xml version=\"1.0\" encoding=\"utf - 8\"?><Screen><Row><Column><Textbox name=\"textbox1\" label=\"Textbox 1\" /></Column></Row><Row><Column><Numericbox name=\"textbox2\" label=\"Numericbox 1\" /></Column></Row><Row><Column><LabelElement name=\"label1\" label=\"Small Label\" type=\"small\" /></Column></Row><Row><Column><LabelElement name=\"label2\" label=\"Medium Label\" type=\"medium\" /></Column></Row><Row><Column><LabelElement name=\"label3\" label=\"Big Label\" type=\"big\" /></Column></Row><Row><Column><Button name=\"button1\" label=\" Click me \" /></Column></Row></Screen>";
        }

        private void btnGerenciar_Click(object sender, RoutedEventArgs e)
        {
            Formularios frm = new Formularios();
            frm.ShowDialog();
            LoadFormularios();
        }

        private void cboFormularios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboFormularios.SelectedItem != null)
            {
                DataRowView drv = cboFormularios.SelectedItem as DataRowView;

                txtXML.Text = drv.Row["xml_formulario"].ToString();
            }
        }

        private void LoadFormularios()
        {
            cboFormularios.ItemsSource = Util.LoadFormularios();
        }
    }
}
