﻿using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Poznamky
{
    public partial class Form1 : Form
    {
        //Globální proměnné pro uložení textu z NameTextBox & NoteTextBox
        private string name;
        private string noteText;
        private DB db = new DB();
        private Poznamka note;
        private int location;
        private int charNumber = 2000;
        private String chars;
        private int size;
        private string username, password;
        private int id_user;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        //ArrayList pro uložení poznámek načtených z tlačítka SendNote_Button
        ArrayList notes = new ArrayList();

        public Form1()
        {
            
            InitializeComponent();
            
            db.db_connect(); //připoj se na dtb
            usernameTextBox.Select();
            
            //Při načtení formu se schová tlačítko pro smazání a smazání všeho
            DeleteNote_Button.Visible = false;
            WrongLogin_Label.Visible = false;
            usernameTextBox.Focus();
            usernameTextBox.Select();
            this.passwordTextBox.PasswordChar = '*';

            this.Close_Button.FlatStyle = FlatStyle.Flat;
            this.Close_Button.FlatAppearance.MouseOverBackColor = Color.Red;
            this.Close_Button.FlatAppearance.MouseDownBackColor = Color.Red;

            this.MyNotes_Button.FlatStyle = FlatStyle.Flat;
            //this.MyNotes_Button.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this.MyNotes_Button.FlatAppearance.MouseDownBackColor = Color.Transparent;

            this.button2.FlatStyle = FlatStyle.Flat;
            //this.button2.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this.button2.FlatAppearance.MouseDownBackColor = Color.Transparent;

            this.button3.FlatStyle = FlatStyle.Flat;
            //this.button3.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this.button3.FlatAppearance.MouseDownBackColor = Color.Transparent;

            this.button4.FlatStyle = FlatStyle.Flat;
            //this.button4.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this.button4.FlatAppearance.MouseDownBackColor = Color.Transparent;

            this.Logout_Button.FlatStyle = FlatStyle.Flat;
            this.Logout_Button.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this.Logout_Button.FlatAppearance.MouseDownBackColor = Color.Transparent;




        }



        public void Form1_Load(object sender, EventArgs e)
        {
            
        }




        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {   
            //Zapisuje text napsaný do NameTextBox-u do proměnné name
            name = NameTextBox.Text;
        }

        private void NoteTextBox_TextChanged(object sender, EventArgs e)
        {
            //Zapisuje text napsaný do NoteTextBox-u do proměnné noteText
            noteText = NoteTextBox.Text;
            NoteTextBox.Focus();



            //Zařizuje počítání znaků do poznámky
            chars = NoteTextBox.Text;
            size  = chars.Length;
            charNumber -= size;
            TextLengthCounter.Text = charNumber.ToString();


            if(charNumber < 1)
            {
                TextLengthCounter.Text = "Chyba";
            }

            charNumber = 2000;

        }



        private void SendNote_Button_Click(object sender, EventArgs e)
        {
            addNote(); //přidej do array listu
            list(); //vypiš
        }

        private void list_load()
        {
            //provádí se na začátku --> načte vše z dtb a vytvoří instance a přidá do array listu notes
            MySqlDataReader reader;
            
            
            reader = db.db_select_notes(id_user);

            while (reader.Read())
            {
                //vytvoř instanci
                note = new Poznamka(reader.GetString("id_note"), reader.GetString("name_note"), reader.GetString("text_note"), reader.GetString("date_note"));
                notes.Add(note); //přidej do array listu
                ShowNote_ListBox.Items.Add("Jméno: " + reader.GetString("name_note") + "    Obsah: " + reader.GetString("text_note"));
            }
            db.db_close(); //ukončení komunikace s dtb zahájené v db.db_select_notes()
        }




        private void list()
        {
            ShowNote_ListBox.Items.Clear(); //vyčisti zobrazené položky
            MySqlDataReader reader;
            reader = db.db_select_notes(id_user); //získej výpis všeho

            while (reader.Read()){
                //vypiš vše
                note = new Poznamka(reader.GetString("id_note"), reader.GetString("name_note"), reader.GetString("text_note"), reader.GetString("date_note"));
                notes.Add(note); //přidej do array listu
                ShowNote_ListBox.Items.Add("Jméno: " + reader.GetString("name_note") + "    Obsah: " + reader.GetString("text_note"));
            }
        }

        private void addNote()
        {
            //Vytvoří nový objekt Poznamka a přidá ho do ArrayListu
            note = new Poznamka(name, noteText);
            db.add_note_db(note, id_user);
            notes.Clear();
            list();
        }





        private void DeleteNote_Button_Click(object sender, EventArgs e)
        {
            //Pokud je v ArrayListu notes poslední poznámka, tak po kliku na tlačítko pro smazání se tyto tlačítka schovají
            if (notes.Count == 1)
            {
                DeleteNote_Button.Visible = false;
            }


            location = ShowNote_ListBox.SelectedIndex;

                
            note = (Poznamka)notes[location];
            int id_note = note.getId();
            db.db_delete_note(id_note);
                
            
            list();
        }

        

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void ShowNote_ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            location = ShowNote_ListBox.SelectedIndex;
            label1.Text = location.ToString();
        }

        private void UpdateNote_Button_Click(object sender, EventArgs e)
        {
            Form_Update form_Update = new Form_Update();



            location = ShowNote_ListBox.SelectedIndex;
            if (location >= 0)
            {
                note = (Poznamka)notes[location];
                form_Update.setId(note.getId());
                form_Update.setName(note.getName());
                form_Update.setNote(note.getText());
                form_Update.Show();
                this.Visible = false;

            }


        }

        private void Close_Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Close_Button_MouseHover(object sender, EventArgs e)
        {
            this.Close_Button.BackColor = Color.Red;
        }

        private void Close_Button_MouseLeave(object sender, EventArgs e)
        {
            this.Close_Button.BackColor = Color.Transparent;
        }

        private void Login_Button_MouseHover(object sender, EventArgs e)
        {
            this.Close_Button.BackColor = Color.AliceBlue;
        }

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {
            username = usernameTextBox.Text;
            
        }

        private void Login_Button_Click(object sender, EventArgs e)
        {
            id_user = -1;
            

            if ((username != null && username.Length < 51) && (password != null && password.Length < 41))
            {
                id_user = db.db_login(username, password); 
            }

            if(id_user != 0 && !(id_user < 0))
            {
                this.WrongLogin_Label.Visible = false;
                LabelUsername.Text = username;
                list_load(); //načti všechen obsah z dtb
                this.LoginPanel.Visible = false;
            } else WrongLogin_Label.Visible = true;

        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            password = passwordTextBox.Text;
        }

        private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Login_Button.PerformClick();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void usernameTextBox_Click(object sender, EventArgs e)
        {
            this.usernameTextBox.Focus();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            EditNote.Visible = true;
            ListNotes.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            list();
            EditNote.Visible = false;
            ListNotes.Visible = true;
        }

        private void usernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Login_Button.PerformClick();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void Logout_Button_Click(object sender, EventArgs e)
        {
            this.LoginPanel.Visible = true;

        }

        private void MyNotes_Button_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            EditNote.Visible = true;
            ListNotes.Visible = false;
        }

        private void Logout_Button_MouseHover(object sender, EventArgs e)
        {
            this.Logout_Button.BackgroundImage = global::Poznamky.Properties.Resources.logout;
        }

        private void Logout_Button_MouseLeave(object sender, EventArgs e)
        {
            this.Logout_Button.BackgroundImage = global::Poznamky.Properties.Resources.logout1;
        }

        private void LogoPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }







    }
    }

