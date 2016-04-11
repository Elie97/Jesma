using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Net.Sockets;
using System.Net;
using System.IO;


namespace SimulationVéhicule
{
    public class Client : Microsoft.Xna.Framework.GameComponent
    {
        TcpClient LeClient { get; set; }
        string IP { get; set; }
        int Port { get; set; }
        int BufferSize { get; set; }
        byte[] ReadBuffer { get; set; }

        MemoryStream ReadStream { get; set; }
        MemoryStream WriteStream { get; set; }

        BinaryReader Reader { get; set; }
        BinaryWriter Writer { get; set; }

        bool EnemyConnected = false;

        List<Voiture> ListeVoiture { get; set; }

        public int NbJoueursConnectés { get; set; }
        public int NbFranchisAdversaire { get; set; }

        public Client(Game game, List<Voiture> listeVoiture)
            : base(game)
        {
            ListeVoiture = listeVoiture;
        }


        public override void Initialize()
        {
            BufferSize = 2048;
            IP = "192.168.2.28";
            Port = 1299;
            LeClient = new TcpClient();
            LeClient.NoDelay = true;
            LeClient.Connect(IP, Port);

            ReadBuffer = new byte[BufferSize];
            ReadStream = new MemoryStream();
            Reader = new BinaryReader(ReadStream);

            WriteStream = new MemoryStream();
            Writer = new BinaryWriter(WriteStream);

            NbJoueursConnectés = 1;

            LeClient.GetStream().BeginRead(ReadBuffer, 0, BufferSize, StreamReceived, null);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public void EnvoiInfoDéplacement(int voiture)
        {
            WriteStream.Position = 0;
            Writer.Write((byte)Protocole.PlayerMoved);
            Writer.Write(ListeVoiture[voiture].NbFranchis);
            Writer.Write(ListeVoiture[voiture].Position.X);
            Writer.Write(ListeVoiture[voiture].Position.Y);
            Writer.Write(ListeVoiture[voiture].Position.Z);
            Writer.Write(ListeVoiture[voiture].Rotation.X);
            Writer.Write(ListeVoiture[voiture].Rotation.Y);
            Writer.Write(ListeVoiture[voiture].Rotation.Z);
            SendData(GetDataFromMemoryStream(WriteStream));
        }

        private void StreamReceived(IAsyncResult ar)//ceci
        {
            int bytesRead = 0;

            try//si pas de data à lire
            {
                lock (LeClient.GetStream())//ep2 4:20
                {
                    bytesRead = LeClient.GetStream().EndRead(ar);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

            if (bytesRead == 0)
            {
                LeClient.Close();//Déconnection
                return;
            }

            byte[] data = new byte[bytesRead];

            for (int i = 0; i < bytesRead; i++)
            {
                data[i] = ReadBuffer[i];
            }

            ProcessData(data);

            LeClient.GetStream().BeginRead(ReadBuffer, 0, BufferSize, StreamReceived, null);
        }

        private void ProcessData(byte[] data)
        {
            ReadStream.SetLength(0);
            ReadStream.Position = 0;

            ReadStream.Write(data, 0, data.Length);
            ReadStream.Position = 0;

            Protocole p;

            try
            {
                p = (Protocole)Reader.ReadByte();

                if (p == Protocole.Connected)
                {
                    string id = Reader.ReadString();
                    string ip = Reader.ReadString();

                    if (!EnemyConnected)
                    {
                        EnemyConnected = true;
                        NbJoueursConnectés++;
                        ListeVoiture[1].Afficher = true;// = new Voiture(this, "MustangGT500SansRoue", 0.0088f, new Vector3(0, 0, 0), new Vector3(-1875, 0, 1200), INTERVALLE_MAJ_STANDARD, false, true);

                        WriteStream.Position = 0;
                        Writer.Write((byte)Protocole.Connected);
                        SendData(GetDataFromMemoryStream(WriteStream));
                    }

                }
                else if (p == Protocole.Disconnected)
                {
                    string id = Reader.ReadString();
                    string ip = Reader.ReadString();
                    NbJoueursConnectés--;
                    EnemyConnected = false;
                }
                else if (p == Protocole.PlayerMoved)
                {
                    NbFranchisAdversaire = Reader.ReadInt32();
                    float px = Reader.ReadSingle();
                    float py = Reader.ReadSingle();
                    float pz = Reader.ReadSingle();
                    float rx = Reader.ReadSingle();
                    float ry = Reader.ReadSingle();
                    float rz = Reader.ReadSingle();
                    string id = Reader.ReadString();
                    string ip = Reader.ReadString();
                    //ListeVoiture[1].Afficher = true;
                    ListeVoiture[1].Position = new Vector3(px, py, pz);
                    ListeVoiture[1].Rotation = new Vector3(rx, ry, rz);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public void SendData(byte[] b)
        {
            //Try to send the data.  If an exception is thrown, disconnect the client
            try
            {
                lock (LeClient.GetStream())
                {
                    LeClient.GetStream().BeginWrite(b, 0, b.Length, null, null);
                }
            }
            catch (Exception e)
            {
            }
        }

        private byte[] GetDataFromMemoryStream(MemoryStream ms)
        {
            byte[] result;

            //Async method called this, so lets lock the object to make sure other threads/async calls need to wait to use it.
            lock (ms)
            {
                int bytesWritten = (int)ms.Position;
                result = new byte[bytesWritten];

                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
            }

            return result;
        }
    }
}
