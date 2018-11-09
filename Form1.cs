﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OPCDA.NET;
using System.Data.SqlClient;

namespace TesteOPC
{
    public partial class Form1 : Form
    {
        string shortcut = "[TCC__]";
        bool lerTag = false;
        string servidorOpc = "";
        List<String> tags = new List<string>();
        string[] itensValor;
        int tempoScan;



        OpcServer srv = new OpcServer();
        OpcGroup oGrp = null;
        OPCItemDef[] items = null;
        OPCItemResult[] addRslt = null;
        int[] err = null;
        object[] val = null;
        OPCItemState[] rslt = null;
        int[] iHnd = null;

        int rtc;

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tb_tempoScan.Text = "1000";
            Int32.TryParse(tb_tempoScan.Text, out tempoScan);
            cb_Servidores.SelectedIndex = 0;


        }



        private void button2_Click(object sender, EventArgs e)
        {

        }



        private void button2_Click_1(object sender, EventArgs e)
        {
            //Cadastra os subcampos da lista na tela com as variáveis do array de resultados
            itensValor = new string[tags.Count];
            for (int i = 0; i < tags.Count; i++)
            {
                itensValor[i] = i.ToString();
                lv_tagList.Items[i].SubItems.Add(itensValor[i]);
            }

            lerTag = true;

            if (servidorOpc == "")
            {
                MessageBox.Show("Nenhum servidor OPC cadastrado");
                return;
            }
            float deadBand = 0.0F;

            try
            {
                //Adicionar um grupo (pasta em que o tag será lido):
                oGrp = srv.AddGroup("Grp1", true, 500, ref deadBand, 0, 0);
            }
            catch
            {
                MessageBox.Show("Erro ao tentar adicionar grupo OPC.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Configurar item para leitura (array)
            items = new OPCItemDef[tags.Count];
            //string tagsArray[55] = tags.ToArray;
            for (int i = 0; i < tags.Count; i++)
            {
                items[i] = new OPCItemDef(tags[i], true, 0, System.Runtime.InteropServices.VarEnum.VT_BSTR);
            }
            addRslt = new OPCItemResult[tags.Count];
            //Adicionar este(s) ite(ns) ao grupo criado anteriormente:
            rtc = oGrp.AddItems(items, out addRslt);
            if (rtc != 0)
            {
                //Se desse erro, o valor de RTC será diferente de 0, aqui dentro poderia ter algum tratamento
            }



            iHnd = new Int32[tags.Count];

            for (int i = 0; i < tags.Count; i++)
            {
                iHnd[i] = addRslt[i].HandleServer;
            }

            //Esse aqui é o evento em paralelo a execução do botão, que faz a leitura contínua do tag:]
            //O seu código está no método logo abaixo, chamado bwLer_DoWork
            if (bwLer.IsBusy == false)
            {
                bwLer.RunWorkerAsync();
            }
        }

        private void bwLer_DoWork(object sender, DoWorkEventArgs e)
        {


            //Esse loop fica em verdadeiro (rodando sempre) quando clico no botão de leitura:
            while (lerTag)
            {
                //Esse método abaixo faz a leitura OPC de todos os tags que eu tiver cadastrado naquele grupo que comenntei no código do botão:
                rtc = oGrp.Read(OPCDA.OPCDATASOURCE.OPC_DS_CACHE, iHnd, out rslt);


                for (int i = 0; i < tags.Count; i++)
                {
                    //Isso aqui é só um tratamento para ver se não veio dado vazio, se vier, escrever erro na tela:
                    if (rslt[i].DataValue == null)
                    {
                        //Condição de erro de leitura
                        this.Invoke(new MethodInvoker(delegate
                        {

                        }));
                    }
                    else
                    {
                        //Se estiver OK o valor, escreve seu valor lido na tela:
                        this.Invoke(new MethodInvoker(delegate
                        {
                            lv_tagList.Items[i].SubItems[1].Text = rslt[i].DataValue.ToString();
                            itensValor[i] = rslt[i].DataValue.ToString();

                            string nomeTabela = "tbValor_" + tags[i].Substring(shortcut.Length);

                            insertDb(nomeTabela, rslt[i].DataValue.ToString());

                        }));

                    }
                }
                System.Threading.Thread.Sleep(tempoScan);
            }
            //Esse método abaixo faz a desconexão com o OPC Server.
            srv.Disconnect();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            //Quando clica para parar de ler, joga o boleano que faz rodar o código contínuo para zero, parando o loop infinito:
            lerTag = false;
        }

        private void tbValor_TextChanged(object sender, EventArgs e)
        {

        }
        private void tbServidor_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void lbTagLeitura_Click(object sender, EventArgs e)
        {

        }
        private void lbServidor_Click(object sender, EventArgs e)
        {

        }

        private void lbServidor_Click_1(object sender, EventArgs e)
        {

        }

        private void tbServidor_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            servidorOpc = cb_Servidores.Text;
            try
            {
                //Tenta conectar no servidor que eu cadastrei:
                rtc = srv.Connect("Localhost", servidorOpc);
                MessageBox.Show("Servidor " + servidorOpc + " cadastrado com sucesso!", "sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lbServidor.Text = servidorOpc;
            }
            catch
            {
                //Se der erro, envio mensagem na tela:
                MessageBox.Show("Erro ao tentar conexão OPC.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                rtc = 12;
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //if (criarTabelaValor(tb_tag.Text))
            //{
            //adiciona o tag na view list e na lista de tags
            criarTabelaValor(tb_tag.Text);
            lv_tagList.Items.Add(new ListViewItem(tb_tag.Text));
            tags.Add(shortcut + tb_tag.Text);
            //}
            //else
            //{
            //    MessageBox.Show("Erro ao criar tabela", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lbValorLe_Click(object sender, EventArgs e)
        {

        }

        private void lbValorLe2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tb_servidor_sql_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //==============================================================================================
        // Persistência (banco de dados)



        private int int_criar_tblUsuario = 0;
        private int int_criar_tblServidores = 0;
        private int int_criar_tblItens = 0;


        private bool insertDb(string nomeTabela, string valor)
        {
            try
            {
                string connectionString2 = @"Data Source=" + tb_servidor_sql.Text + ";Initial Catalog=Ilogger;User ID=" + tb_usuario_sql.Text + ";Password=" + tb_senha_sql.Text;
                SqlConnection con2 = new SqlConnection(connectionString2);
                SqlCommand cmd2 = new SqlCommand("INSERT INTO [Ilogger].[dbo].[" + nomeTabela + "] (valor) VALUES (" + valor + ");", con2);
                SqlDataReader myReader2;
                con2.Open();
                myReader2 = cmd2.ExecuteReader();
                con2.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private void btn_testar_conexao_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=" + tb_servidor_sql.Text + ";" +
          "User ID=" + tb_usuario_sql.Text + ";" +
          "Password=" + tb_senha_sql.Text;

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                lb_serverStatus.Text = "Conexão Realizada com Sucesso!";
                lb_serverStatus.ForeColor = Color.Green;
                con.Close();
            }
            catch (Exception)
            {
                lb_serverStatus.Text = "Falhou Conexão";
                lb_serverStatus.ForeColor = Color.Red;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=" + tb_servidor_sql.Text + ";User ID=" + tb_usuario_sql.Text + ";Password=" + tb_senha_sql.Text;
            SqlConnection con1 = new SqlConnection(connectionString);
            SqlCommand cmd1 = new SqlCommand("create database Ilogger;", con1);

            SqlDataReader myReader;
            try
            {
                con1.Open();
                myReader = cmd1.ExecuteReader();
                con1.Close();
                MessageBox.Show("O banco de dados Ilogger foi criado com sucesso!");
                lb_nomeDoBanco.Text = "Ilogger";


                try // Criando a Tabela  Usuários
                {
                    string connectionString2 = @"Data Source=" + tb_servidor_sql.Text + ";Initial Catalog=Ilogger;User ID=" + tb_usuario_sql.Text + ";Password=" + tb_senha_sql.Text;
                    SqlConnection con2 = new SqlConnection(connectionString2);
                    SqlCommand cmd2 = new SqlCommand("CREATE TABLE [dbo].[tblUsuarios]([login] [varchar](50) NOT NULL, [senha] [varchar](25) NOT NULL, [data_criacao] [date] NOT NULL, [data_ultimo_login] [date] NULL, [cargo] [varchar](50) NULL, CONSTRAINT [PK_tblUsuarios] PRIMARY KEY CLUSTERED ([login] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ) ON [PRIMARY]; SET ANSI_PADDING OFF; ALTER TABLE [dbo].[tblUsuarios] ADD  CONSTRAINT [DF_tblUsuarios_data_criacao]  DEFAULT (getdate()) FOR [data_criacao];", con2);

                    SqlDataReader myReader2;

                    con2.Open();
                    myReader2 = cmd2.ExecuteReader();
                    con2.Close();
                    int_criar_tblUsuario = 1;
                }
                catch (Exception)
                {
                    MessageBox.Show("A tabela Usuários não foi criada.");
                } // Fim da criação da Tabela Usuários


                try // Criando a Tabela Servidores
                {
                    string connectionString2 = @"Data Source=" + tb_servidor_sql.Text + ";Initial Catalog=Ilogger;User ID=" + tb_usuario_sql.Text + ";Password=" + tb_senha_sql.Text;
                    SqlConnection con2 = new SqlConnection(connectionString2);
                    SqlCommand cmd2 = new SqlCommand("CREATE TABLE [dbo].[tblServidores]([nome_servidor] [varchar](100) NOT NULL, [status_conexao] [int] NULL, CONSTRAINT [PK_tblServidores] PRIMARY KEY CLUSTERED ([nome_servidor] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]", con2);
                    SqlDataReader myReader2;

                    con2.Open();
                    myReader2 = cmd2.ExecuteReader();
                    con2.Close();
                    int_criar_tblUsuario = 1;
                }
                catch (Exception)
                {
                    MessageBox.Show("A tabela Servidores não foi criada.");
                } // Fim da criação da Tabela Servidores


                try // Criando a Tabela Itens
                {
                    string connectionString2 = @"Data Source=" + tb_servidor_sql.Text + ";Initial Catalog=Ilogger;User ID=" + tb_usuario_sql.Text + ";Password=" + tb_senha_sql.Text;
                    SqlConnection con2 = new SqlConnection(connectionString2);
                    SqlCommand cmd2 = new SqlCommand("CREATE TABLE [dbo].[tblItens]([id] [int] IDENTITY(1,1) NOT NULL, [endereco_item] [varchar](100) NOT NULL,	[tag] [varchar](50) NOT NULL, [servidor] [varchar](100) NOT NULL, [tipo_valor] [varchar](15) NOT NULL, CONSTRAINT [PK_tblItens] PRIMARY KEY CLUSTERED ([endereco_item] ASC, [servidor] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]", con2);
                    SqlDataReader myReader2;

                    con2.Open();
                    myReader2 = cmd2.ExecuteReader();
                    con2.Close();
                    int_criar_tblUsuario = 1;
                }
                catch (Exception)
                {
                    MessageBox.Show("A tabela Itens não foi criada.");
                } // Fim da criação da Tabela Itens

            }
            catch (Exception)
            {
                MessageBox.Show("Não foi possível criar o banco de dados.");
            }

            criarTabelasDinamicas();
            MessageBox.Show("Foram criadas as tabelas: " + verificaTabelas());
        }

        //criando as tabelas dinamicamente de acordo com as variáveis previamente cadastradas

        private void criarTabelasDinamicas()
        {
            foreach (string tag in tags)
            {
                criarTabelaValor(tag.Substring(shortcut.Length));
            }
        }


        private bool criarTabelaValor(string nomeTabela)
        {

            try
            {

                string connectionString2 = @"Data Source=" + tb_servidor_sql.Text + ";Initial Catalog=Ilogger;User ID=" + tb_usuario_sql.Text + ";Password=" + tb_senha_sql.Text;
                SqlConnection con2 = new SqlConnection(connectionString2);
                SqlCommand cmd2 = new SqlCommand("CREATE TABLE [dbo].[tbValor_" + nomeTabela + "]([data_time] [datetime] NOT NULL,[valor] [varchar](20) NOT NULL) ON [PRIMARY]; ALTER TABLE [dbo].[tbValor_" + nomeTabela + "] ADD  CONSTRAINT [tbValor_" + nomeTabela + "_data_time]  DEFAULT (getdate()) FOR [data_time];", con2);
                SqlDataReader myReader2;
                con2.Open();
                myReader2 = cmd2.ExecuteReader();
                con2.Close();

                return true;
            }
            catch (Exception)
            {

                MessageBox.Show("Não foi possível criar a tabela: " + nomeTabela);
                return false;
                throw;
            }

        }

        private string verificaTabelas()
        {
            try
            {

                string connectionString2 = @"Data Source=" + tb_servidor_sql.Text + ";Initial Catalog=Ilogger;User ID=" + tb_usuario_sql.Text + ";Password=" + tb_senha_sql.Text;
                SqlConnection con2 = new SqlConnection(connectionString2);
                SqlCommand cmd2 = new SqlCommand("use Ilogger;select TABLE_NAME from INFORMATION_SCHEMA.TABLES", con2);
                SqlDataReader myReader2;
                con2.Open();
                myReader2 = cmd2.ExecuteReader();
                con2.Close();
                return myReader2.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao verificar tabelas");
                return null;
                throw;
            }
        }


        private void label9_Click(object sender, EventArgs e)
        {


        }

        private void tb_tempoScan_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            Int32.TryParse(tb_tempoScan.Text, out tempoScan);
        }

        private void tb_tempoScan_ValueChanged(object sender, EventArgs e)
        {

        }
    }





}
