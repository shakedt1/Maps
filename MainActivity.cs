using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Gms.Common.Apis.Internal;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Java.Lang;
using System;
using System.ComponentModel;

namespace useMap
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private GoogleMap map;
        Random rnd = new Random();

        LatLng latLng = new LatLng(32.783092, 35.510874);

        private BackgroundWorker worker = new BackgroundWorker();

        MarkerOptions marker = new MarkerOptions().SetPosition(new LatLng(32.783092, 35.510874));
       

        public readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

        private Button btnStart;
        private Button btnRestart;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            btnStart = FindViewById<Button>(Resource.Id.startBtn);
            btnRestart = FindViewById<Button>(Resource.Id.restartBtn);

            setUpMap();

            worker.DoWork += Worker_DoWork; 
            worker.ProgressChanged += UserStateWorker_ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            
            btnStart.Click += BtnStart_Click;
            btnRestart.Click += BtnRestart_Click;
        }

        private void UserStateWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            map.Clear();
            MarkerOptions marker1 = new MarkerOptions().SetPosition((LatLng)e.UserState);
            map.AddMarker(marker1);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (btnStart.Text == "Stop")
            {
                switch (rnd.Next(1, 5))
                {
                    case 1:
                        latLng.Latitude = latLng.Latitude + 0.001;
                        break;
                    case 2:
                        latLng.Latitude = latLng.Latitude - 0.001;
                        break;
                    case 3:
                        latLng.Longitude = latLng.Longitude + 0.001;
                        break;
                    case 4:
                        latLng.Longitude = latLng.Longitude - 0.001;
                        break;
                }
                worker.ReportProgress(0, latLng);
                Thread.Sleep(1000);
            }
            e.Cancel = true;
        }

        private void BtnRestart_Click(object sender, System.EventArgs e)
        {
            map.Clear();
            map.AddMarker(marker);
            latLng = new LatLng(32.783092, 35.510874);
        }

        private void BtnStart_Click(object sender, System.EventArgs e)
        {
            if (btnStart.Text == "Start")
            {
                btnStart.Text = "Stop";
                btnRestart.Visibility = Android.Views.ViewStates.Invisible;
                worker.RunWorkerAsync();

            }
            else
            {
                btnStart.Text = "Start";
                btnRestart.Visibility = Android.Views.ViewStates.Visible;
            }
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private void setUpMap()
        {
            if (map == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(32.783092, 35.510874), 15);
            map.MoveCamera(cameraUpdate);

            map.AddMarker(marker);

        }

    }
}