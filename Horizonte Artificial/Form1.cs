using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Horizonte_Artificial
{
    public partial class Form1 : Form
    {
        //inicializar el bitmap
        Bitmap horizonte;
        Graphics grafico;
        SolidBrush pincelBlanco, pincelRojo, pincelTierra, pincelCielo;
        Pen lapizBlanco, lapizRojo, lapizTierra, lapizCielo;
        Font Arial;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Dibujar(50, -1);
        }

        private void Dibujar(int pitch, int roll)
        {
            //Las variables PPitch y ARoll son las que se deben modificar para mover al horizonte
                                            #region Config
            /**** Configuracion ******/
            //Reescalado
            int scl;
            if (pictureBox1.Width <= pictureBox1.Height) { scl = pictureBox1.Width; }
            else { scl = pictureBox1.Height; }

            //Valor de pitch
            int Vpitch = 0;  //el valor de pitch tiene su 0 en el centro del instrumento y toma valores pos hacia abajo
            //Angulo de roll
            double ARoll= 0; //se lo inicializa a 0

            //centro de la pantalla
            Point centro = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2); //punto central
            //Puntos del rectangulo de gnd
            Point[] gnd = new Point[4];
            //Puntos lineas largas
            Point a = new Point(); //primer punto de la linea
            Point b = new Point(); // segundo
            Point t = new Point(); //punto del texto
            /* 
             * Se utilizan dos puntos en relacion a las lineas de pitch, centLineas se utilizara como el punto central
             * sobre el cual rotara cada linea (se varia en el for) y PPitch es el punto central de todas las lineas de pitch
             * (se podria decir que es el punto central de la linea 0) que se vera afectado por la variacion de pitch y yaw
             */
            //Punto central de las lineas
            Point centLineas = new Point(centro.X, centro.Y); //Lo inicializo en el mismo lugar que el centro de la pantalla
            //Punto central de las lineas
            Point PPitch = new Point(centro.X, centro.Y);
            //Colores para el dibujo
            Color cielo = Color.SteelBlue;
            Color tierra = Color.Chocolate;
            
            //Rectangulo de gnd
            int adjH = 180; //Angulo de ajuste para lineas horizontales
            int adjV = 0; //Angulo de ajuste para lineas verticales
            int lngGnd = scl * 300;
            
            //Lineas
            int lngLinL = scl / 50; //Longitud de las lineas largas
            
            //Grosor de las lineas
            float grosor = scl * 3 / 300; //El 300 se usa porque el bitmap original se hizo con un tamaño de 300px

            //Fuente
            int fuente = scl / 16;
            if (fuente < 1) { fuente = 1; } //Evita que la fuente sea menor a 1

            //Inicializacion
            horizonte = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            grafico = Graphics.FromImage(horizonte);
                //Brushes
            pincelBlanco = new SolidBrush(Color.White);
            pincelRojo = new SolidBrush(Color.Red);
            pincelTierra = new SolidBrush(tierra);
            pincelCielo = new SolidBrush(cielo); //Teal, Aqua
                //Lapices
            lapizBlanco = new Pen(pincelBlanco, grosor);
            lapizRojo = new Pen(pincelRojo, grosor);
            lapizTierra = new Pen(pincelTierra, grosor);
            lapizCielo = new Pen(pincelCielo, grosor);

            Arial = new Font("Arial", fuente);
                                            #endregion

            /*      VALORES A MANEJAR 
             *  -pitch -> Movimiento hacia arriba y abajo del gnd
             *  -yaw   -> creo que no lo necesito
             *  -roll  -> Giro del gnd
             */

            /******** Comandos *****************/
            if(pitch != -1)
                Vpitch = pitch - 50; //conversion adaptada al VScroll

            PPitch.X = pictureBox1.Width / 2; //por ahora constante

            PPitch.Y = Vpitch * (pictureBox1.Height / 2) / 50 + pictureBox1.Height /2; //El 50 representa el valor de pitch correspondiente  a pictureBox1.Height / 2

            if (roll != -1)
                ARoll = roll * 360 / 100;

            /******** Dibujo Base del Horizonte *********/
            grafico.Clear(cielo);
            //dibujar rectangulo del suelo
                //Rectangulo
            gnd[0].X = (int)(centro.X + (lngGnd * Math.Cos((adjH + 180 + ARoll) * Math.PI / 180)));
            gnd[0].Y = (int)(centro.Y + (lngGnd * Math.Sin((adjH + 180 + ARoll) * Math.PI / 180)));
            gnd[3].X = (int)(centro.X + (lngGnd * Math.Cos((adjH + ARoll) * Math.PI / 180)));
            gnd[3].Y = (int)(centro.Y + (lngGnd * Math.Sin((adjH + ARoll) * Math.PI / 180)));
            gnd[1].X = (int)(centro.X + (lngGnd * Math.Cos((adjH + 180 + ARoll) * Math.PI / 180)));
            gnd[1].Y = (int)((centro.Y + pictureBox1.Height / 2 * scl) + (lngGnd * Math.Sin((adjH + 180 + ARoll) * Math.PI / 180)));
            gnd[2].X = (int)(centro.X + (lngGnd * Math.Cos((adjH + ARoll) * Math.PI / 180)));
            gnd[2].Y = (int)((centro.Y + pictureBox1.Height / 2 * scl) + (lngGnd * Math.Sin((adjH + ARoll) * Math.PI / 180)));
            //Con esto funciona hasta los 90 grados de giro

                //Dibujo
            grafico.DrawPolygon(lapizBlanco, gnd);
            grafico.FillPolygon(pincelTierra, gnd);
            
            /*********** Dibujo lineas de altitud *******************/

            //Lineas largas
            int sepLin = scl / 4; //Factor de separacion de las lineas

            for (int i = 0; i < 18; i++)
            {
                centLineas.X = (int)(PPitch.X - (i * sepLin * Math.Cos((adjV + ARoll) * Math.PI / 180)));
                centLineas.Y = (int)(PPitch.Y - (i * sepLin * Math.Sin((adjV + ARoll) * Math.PI / 180)));

                a.X = (int)(centLineas.X + (lngLinL * Math.Cos((adjH + 180 + ARoll) * Math.PI / 180)));
                a.Y = (int)(centLineas.Y + (lngLinL * Math.Sin((adjH + 180 + ARoll) * Math.PI / 180)));

                b.X = (int)(centLineas.X + (lngLinL * Math.Cos((adjH + ARoll) * Math.PI / 180)));
                b.Y = (int)(centLineas.Y + (lngLinL * Math.Sin((adjH + ARoll) * Math.PI / 180)));

                grafico.DrawLine(lapizBlanco, a, b);

                
                //t.Y = a.Y - scl / 20;
                //if (i == 0) { t.X += scl / 40; } //Ajuste de 0
                //grafico.DrawString((i * 10).ToString(), Arial, pincelBlanco, t);
                //if (i == 0) { t.X -= scl / 40; }
            }
            t.X -= scl / 60; //ajuste por el '-'
            for(int i = 1; i < 17; i++)
            {
                //a.Y = b.Y = PPitch.Y + i * scl / 4;
                //grafico.DrawLine(lapizBlanco, a, b);

                //t.Y = a.Y - scl / 20;
                //grafico.DrawString("-" + (i * 10).ToString(), Arial, pincelBlanco, t);
            }

            /******* Dibujo reticula central (fija) ************/

            //Punto central
            Point puntoCentral = new Point(); //Punto inicial del rectangulo que contiene al punto central
            puntoCentral.X = pictureBox1.Width / 2 - (scl / 150);
            puntoCentral.Y = pictureBox1.Height / 2 - (scl / 150); //Escalado
            Rectangle recCentral = new Rectangle(puntoCentral.X, puntoCentral.Y, scl / 75, scl / 75);
            grafico.DrawEllipse(lapizRojo, recCentral);
            grafico.FillEllipse(pincelRojo, recCentral);

            //Lineas
            grafico.DrawLine(lapizRojo, centro.X - scl / 4, centro.Y, centro.X - scl / 20, centro.Y);
            grafico.DrawLine(lapizRojo, pictureBox1.Width - scl / 4, centro.Y, centro.X + scl / 20, centro.Y);
            
            //Circulo



                //IMPORTANTE colocar la imagen creada en el picturebox al final de la funcion
                pictureBox1.Image = horizonte;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Dibujar(vScrollBar1.Value, -1);
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Dibujar(-1, hScrollBar1.Value);
        }

    }
}
