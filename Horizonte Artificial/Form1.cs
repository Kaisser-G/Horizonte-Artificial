/*
 * -Dibujar las lineas de abajo
 * -Crear las lineas chicas
 * -Hacer la variacion de pitch en angulo  ---listo
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Horizonte_Artificial
{
    public partial class Form1 : Form
    {
        //inicializar el bitmap
        Bitmap horizonte;
        Graphics grafico;

        SolidBrush pincelBlanco, pincelRojo, pincelNegro, pincelGris, pincelTierra, pincelCielo;
        Pen lapizBlanco, lapizRojo, lapizNegro, lapizGris, lapizTierra, lapizCielo;
        Font Arial;


        Datos datos = new Datos();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            datos.Pitch = 50;
            datos.Roll = 0;
            datos.Yaw = 50;
            Dibujar(datos.Pitch, datos.Roll, datos.Yaw);
        }

        private void Dibujar(int pitch, int roll, int yaw)
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
            //Valor del Yaw
            int VYaw = 0;
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
            //Punto central de cada linea
            Point centLineas = new Point(centro.X, centro.Y); //Lo inicializo en el mismo lugar que el centro de la pantalla
            //Punto central
            Point PCentral = new Point(centro.X, centro.Y);
            //Colores para el dibujo
            Color cielo = Color.SteelBlue;
            Color tierra = Color.Chocolate;
            
            //Rectangulo de gnd
            int adjH = 180; //Angulo de ajuste para lineas horizontales
            int adjV = 90; //Angulo de ajuste para lineas verticales
            int lngGnd = scl * 300;
            
            //Lineas
            int lngLinL = scl / 5; //Longitud de las lineas largas
            int lngLinC = scl / 8; //Longitud de las lineas cortas
            int sepLin = scl / 4; //Factor de separacion de las lineas
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
            pincelNegro = new SolidBrush(Color.Black);
            pincelGris = new SolidBrush(Color.LightGray);
            pincelTierra = new SolidBrush(tierra);
            pincelCielo = new SolidBrush(cielo); //Teal, Aqua
                //Lapices
            lapizBlanco = new Pen(pincelBlanco, grosor);
            lapizRojo = new Pen(pincelRojo, grosor);
            lapizNegro = new Pen(pincelNegro, grosor);
            lapizGris = new Pen(pincelGris, grosor);
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
            
            Vpitch = pitch - 50; //conversion adaptada al VScroll

            VYaw = yaw - 50; //conversion adaptada al VScroll
            
            ARoll = roll * 360 / 100;


            PCentral.X = (int)(centro.X + (Vpitch * Math.Cos((ARoll + adjV) * Math.PI / 180)));
            PCentral.Y = (int)(centro.Y + (Vpitch * Math.Sin((ARoll + adjV) * Math.PI / 180 )));

            PCentral.X = (int)(PCentral.X + (VYaw * Math.Cos((ARoll) * Math.PI / 180)));
            PCentral.Y = (int)(PCentral.Y + (VYaw * Math.Sin((ARoll) * Math.PI / 180)));

            //PPitch.X = pictureBox1.Width / 2; //por ahora constante
            //PPitch.Y = Vpitch * (pictureBox1.Height / 2) / 50 + pictureBox1.Height /2; //El 50 representa el valor de pitch correspondiente  a pictureBox1.Height / 2


            /******** Dibujo Base del Horizonte *********/
            grafico.Clear(cielo);
            //dibujar rectangulo del suelo
            Point baseRec = new Point();
            baseRec.X = (int)(centro.X + (scl * 2 * Math.Cos((ARoll + adjV) * Math.PI / 180)));
            baseRec.Y = (int)(centro.Y + (scl * 2 * Math.Sin((ARoll + adjV) * Math.PI / 180)));

                //Rectangulo
            gnd[0].X = (int)(centro.X + (lngGnd * Math.Cos((180 + ARoll) * Math.PI / 180)));
            gnd[0].Y = (int)(centro.Y + (lngGnd * Math.Sin((180 + ARoll) * Math.PI / 180)));
            gnd[1].X = (int)(centro.X + (lngGnd * Math.Cos((ARoll) * Math.PI / 180)));
            gnd[1].Y = (int)(centro.Y + (lngGnd * Math.Sin((ARoll) * Math.PI / 180)));
            gnd[2].X = (int)(baseRec.X + (lngGnd * Math.Cos((ARoll) * Math.PI / 180)));
            gnd[2].Y = (int)(baseRec.Y + (lngGnd * Math.Sin((180 + ARoll) * Math.PI / 180)));
            gnd[3].X = (int)(baseRec.X + (lngGnd * Math.Cos((ARoll) * Math.PI / 180)));
            gnd[3].Y = (int)(baseRec.Y + (lngGnd * Math.Sin((ARoll) * Math.PI / 180)));
            //Con esto funciona hasta los 90 grados de giro

                //Dibujo
            grafico.DrawPolygon(lapizBlanco, gnd);
            grafico.FillPolygon(pincelTierra, gnd);
            
            /*********** Dibujo lineas de altitud *******************/

            //Lineas largas
                //Texto
            Size textoChico = new Size(scl / 8, scl / 8);
            Size textoGrande = new Size(scl / 6, scl / 6);

            PointF[] recGrande = new PointF[4];

            int sepTxt = scl / 30; //Separacion del texto con respecto a las lineas
            int txtWidth = scl / 40; //Anchura del cuadro de texto
            int txtHeight = scl / 80; //Mitad de la altura del cuadro de texto
                //Lineas superiores
            for (int i = 0; i < 18; i++)
            {
                centLineas.X = (int)(PCentral.X + (i * sepLin * Math.Cos((adjV - ARoll) * Math.PI / 180)));
                centLineas.Y = (int)(PCentral.Y - (i * sepLin * Math.Sin((adjV - ARoll) * Math.PI / 180)));

                a.X = (int)(centLineas.X + (lngLinL * Math.Cos((180 + ARoll) * Math.PI / 180)));
                a.Y = (int)(centLineas.Y + (lngLinL * Math.Sin((180 + ARoll) * Math.PI / 180)));

                b.X = (int)(centLineas.X + (lngLinL * Math.Cos((ARoll) * Math.PI / 180)));
                b.Y = (int)(centLineas.Y + (lngLinL * Math.Sin((ARoll) * Math.PI / 180)));

                grafico.DrawLine(lapizBlanco, a, b);

                //Para hacer el texto voy a tener que crear una caja y ubicarlo dentro
                //recGrande[0].X = (float)(a.X - (sepTxt * Math.Cos((ARoll) * Math.PI / 180)));
                //recGrande[0].Y = (float)(a.Y - (txtHeight * Math.Sin((ARoll) * Math.PI / 180)));
                //recGrande[1].X = (float)(recGrande[0].X + (txtWidth * Math.Cos((ARoll) * Math.PI / 180)));
                //recGrande[1].Y = recGrande[0].Y;
                //recGrande[2].X = (float)(a.X - (sepTxt * Math.Cos((ARoll) * Math.PI / 180)));
                //recGrande[2].Y = (float)(a.Y + (txtHeight * Math.Sin((ARoll) * Math.PI / 180)));
                //recGrande[3].X = (float)(recGrande[2].X + (txtWidth * Math.Cos((ARoll) * Math.PI / 180)));
                //recGrande[3].Y = recGrande[2].Y;

                //t.Y = a.Y - scl / 20;
                //if (i == 0) { t.X += scl / 40; } //Ajuste de 0
                //grafico.DrawString((i * 10).ToString(), Arial, pincelBlanco, t);
                //if (i == 0) { t.X -= scl / 40; }
            }

            t.X -= scl / 60; //ajuste por el '-'
                //Lineas inferores
            for(int i = 1; i < 17; i++)
            {
                centLineas.X = (int)(PCentral.X - (i * sepLin * Math.Cos((adjV - ARoll) * Math.PI / 180)));
                centLineas.Y = (int)(PCentral.Y + (i * sepLin * Math.Sin((adjV - ARoll) * Math.PI / 180)));

                a.X = (int)(centLineas.X + (lngLinL * Math.Cos((180 + ARoll) * Math.PI / 180)));
                a.Y = (int)(centLineas.Y + (lngLinL * Math.Sin((180 + ARoll) * Math.PI / 180)));

                b.X = (int)(centLineas.X + (lngLinL * Math.Cos((ARoll) * Math.PI / 180)));
                b.Y = (int)(centLineas.Y + (lngLinL * Math.Sin((ARoll) * Math.PI / 180)));

                grafico.DrawLine(lapizBlanco, a, b);
            }

            //Lineas cortas
                //Lineas Superiores
            for (int i = 0; i < 18; i++)
            {
                centLineas.X = (int)(PCentral.X + ((i * sepLin - (sepLin / 2)) * Math.Cos((adjV - ARoll) * Math.PI / 180)));
                centLineas.Y = (int)(PCentral.Y - ((i * sepLin - (sepLin / 2))* Math.Sin((adjV - ARoll) * Math.PI / 180)));

                a.X = (int)(centLineas.X + (lngLinC * Math.Cos((180 + ARoll) * Math.PI / 180)));
                a.Y = (int)(centLineas.Y + (lngLinC * Math.Sin((180 + ARoll) * Math.PI / 180)));

                b.X = (int)(centLineas.X + (lngLinC * Math.Cos((ARoll) * Math.PI / 180)));
                b.Y = (int)(centLineas.Y + (lngLinC * Math.Sin((ARoll) * Math.PI / 180)));

                grafico.DrawLine(lapizBlanco, a, b);

            }

            t.X -= scl / 60; //ajuste por el '-'
                //Lineas inferores
            for (int i = 1; i < 17; i++)
            {
                centLineas.X = (int)(PCentral.X - ((i * sepLin + (sepLin / 2)) * Math.Cos((adjV - ARoll) * Math.PI / 180)));
                centLineas.Y = (int)(PCentral.Y + ((i * sepLin + (sepLin / 2)) * Math.Sin((adjV - ARoll) * Math.PI / 180)));

                a.X = (int)(centLineas.X + (lngLinC * Math.Cos((180 + ARoll) * Math.PI / 180)));
                a.Y = (int)(centLineas.Y + (lngLinC * Math.Sin((180 + ARoll) * Math.PI / 180)));

                b.X = (int)(centLineas.X + (lngLinC * Math.Cos((ARoll) * Math.PI / 180)));
                b.Y = (int)(centLineas.Y + (lngLinC * Math.Sin((ARoll) * Math.PI / 180)));

                grafico.DrawLine(lapizBlanco, a, b);
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
            grafico.DrawArc(lapizRojo, centro.X - scl / 20, centro.Y - scl / 20, scl / 10, scl / 10, 0, 180);


            //Borde del instrumento
            grafico.DrawEllipse(lapizBlanco, 0, 0, pictureBox1.Width, pictureBox1.Height); //Creo el borde del horizonte
            GraphicsPath area = new GraphicsPath(); //Creo un Path, que es una serie de segmentos, que en este caso utilizo como el area a rellenar
            area.AddEllipse(0, 0, pictureBox1.Width, pictureBox1.Height); //Agrego el circulo interior
            area.AddEllipse(-(pictureBox1.Width / 2), -(pictureBox1.Height / 2), pictureBox1.Width * 2, pictureBox1.Height * 2); //Agrego un circulo exterior mas grande
            grafico.FillPath(pincelNegro, area); //Relleno el area creada

            //Referencia de horizonte
            int altTrg = scl / 10; //Mitad de la altura del triangulo de referencia
            int lrgTrg = scl / 7; //Anchura de los triangulos de referecia

            Point[] trgL = new Point[3]; //Triangulo izquierdo
            Point[] trgR = new Point[3]; //Triangulo derecho

            trgL[0].X = trgL[2].X = 0;
            trgL[1].X = lrgTrg;
            trgL[0].Y = centro.Y - altTrg;
            trgL[1].Y = centro.Y;
            trgL[2].Y = centro.Y + altTrg;

            grafico.DrawPolygon(lapizNegro, trgL);
            grafico.FillPolygon(pincelGris, trgL);

            trgR[0].X = trgR[2].X = pictureBox1.Width;
            trgR[1].X = pictureBox1.Width - lrgTrg;
            trgR[0].Y = centro.Y - altTrg;
            trgR[1].Y = centro.Y;
            trgR[2].Y = centro.Y + altTrg;

            grafico.DrawPolygon(lapizNegro, trgR);
            grafico.FillPolygon(pincelGris, trgR);

            //Referencia de Viraje
            double sepBrg = scl / 2; //Separacion con respecto al centro de las lineas de viraje
            Point[] brgL = new Point[2];
            Point[] brgR = new Point[2];

            brgR[0].X = (int)(centro.X + (sepBrg * Math.Cos((-50) * Math.PI / 180)));
            brgR[0].Y = (int)(centro.Y + (sepBrg * Math.Sin((-50) * Math.PI / 180)));
            brgR[1].X = (int)(centro.X + (sepBrg * 2 * Math.Cos((-50) * Math.PI / 180)));
            brgR[1].Y = (int)(centro.Y + (sepBrg * 2 * Math.Sin((-50) * Math.PI / 180)));

            brgL[0].X = (int)(centro.X + (sepBrg * Math.Cos((-130) * Math.PI / 180)));
            brgL[0].Y = (int)(centro.Y + (sepBrg * Math.Sin((-130) * Math.PI / 180)));
            brgL[1].X = (int)(centro.X + (sepBrg * 2 * Math.Cos((-130) * Math.PI / 180)));
            brgL[1].Y = (int)(centro.Y + (sepBrg * 2 * Math.Sin((-130) * Math.PI / 180)));

            grafico.DrawLine(lapizBlanco, brgR[0], brgR[1]);
            grafico.DrawLine(lapizBlanco, brgL[0], brgL[1]);

                //IMPORTANTE colocar la imagen creada en el picturebox al final de la funcion
                pictureBox1.Image = horizonte;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Dibujar(vScrollBar1.Value, datos.Roll, datos.Yaw);
            datos.Pitch = vScrollBar1.Value;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Dibujar(datos.Pitch, hScrollBar1.Value - 50, datos.Yaw); //Le resto 50 para ajustar el valor a 0
            label1.Text = ((hScrollBar1.Value - 50) * 360 / 100).ToString();
            datos.Roll = hScrollBar1.Value - 50;
        }

        private void hScrollBarYaw_Scroll(object sender, ScrollEventArgs e)
        {
            Dibujar(datos.Pitch, datos.Roll, hScrollBarYaw.Value);
            datos.Yaw = hScrollBarYaw.Value;
        }


    }
}
