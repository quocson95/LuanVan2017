using System;
using System.Collections.Generic;
using System.Net;
using Android.App;
using Android.Graphics;
using Android.Support.V4.Graphics.Drawable;
using Android.Views;
using Android.Widget;
using Xamarin.Auth;

namespace FreeHand.Message.Mail
{    
    public class ListAccountAdapter : BaseAdapter<User>
    {
        IList<Tuple<User, Account>> items;
        Activity context;
        public ListAccountAdapter(Activity context, IList<Tuple<User,Account>> items): base()
        {
                   this.context = context;
                   this.items = items;
        }
        public override User this[int position] 
        {
            get { return items[position].Item1; }
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.listAccount, null);
            TextView name_login = view.FindViewById<TextView>(Resource.Id.name_login);
            TextView nameDisplay = view.FindViewById<TextView>(Resource.Id.nameDisplay);
            Switch swActive = view.FindViewById<Switch>(Resource.Id.sw_active);
            ImageView avatar = view.FindViewById<ImageView>(Resource.Id.img_avatar);

            name_login.Text = item.Item1.Email;
            nameDisplay.Text = item.Item1.Name;
            if (!string.IsNullOrEmpty(item.Item1.Picture))
            {
                var imageBitmap = GetImageBitmapFromUrl(item.Item1.Picture);
                if (imageBitmap != null)
                {
                    RoundedBitmapDrawable drawable = createRoundedBitmapDrawableWithBorder(imageBitmap);
                    avatar.SetImageDrawable(drawable);
                }
            }
           
            //swActive.Selected = item.GetActive();
            return view;;
        }

        private RoundedBitmapDrawable createRoundedBitmapDrawableWithBorder(Bitmap bitmap)
        {
            int bitmapWidth = bitmap.Width;
            int bitmapHeight = bitmap.Height;
            int borderWidthHalf = 10; // In pixels
                                      //Toast.makeText(mContext,""+bitmapWidth+"|"+bitmapHeight,Toast.LENGTH_SHORT).show();

            // Calculate the bitmap radius
            int bitmapRadius = Math.Min(bitmapWidth, bitmapHeight) / 2;

            int bitmapSquareWidth = Math.Min(bitmapWidth, bitmapHeight);
            //Toast.makeText(mContext,""+bitmapMin,Toast.LENGTH_SHORT).show();

            int newBitmapSquareWidth = bitmapSquareWidth + borderWidthHalf;
            //Toast.makeText(mContext,""+newBitmapMin,Toast.LENGTH_SHORT).show();

            /*
                Initializing a new empty bitmap.
                Set the bitmap size from source bitmap
                Also add the border space to new bitmap
            */
            Bitmap roundedBitmap = Bitmap.CreateBitmap(newBitmapSquareWidth, newBitmapSquareWidth, Bitmap.Config.Argb8888);

            /*
                Canvas
                    The Canvas class holds the "draw" calls. To draw something, you need 4 basic
                    components: A Bitmap to hold the pixels, a Canvas to host the draw calls (writing
                    into the bitmap), a drawing primitive (e.g. Rect, Path, text, Bitmap), and a paint
                    (to describe the colors and styles for the drawing).

                Canvas(Bitmap bitmap)
                    Construct a canvas with the specified bitmap to draw into.
            */
            // Initialize a new Canvas to draw empty bitmap
            Canvas canvas = new Canvas(roundedBitmap);

            /*
                drawColor(int color)
                    Fill the entire canvas' bitmap (restricted to the current clip) with the specified
                    color, using srcover porterduff mode.
            */
            // Draw a solid color to canvas
            canvas.DrawColor(Color.Red);

            // Calculation to draw bitmap at the circular bitmap center position
            int x = borderWidthHalf + bitmapSquareWidth - bitmapWidth;
            int y = borderWidthHalf + bitmapSquareWidth - bitmapHeight;

            /*
                drawBitmap(Bitmap bitmap, float left, float top, Paint paint)
                    Draw the specified bitmap, with its top/left corner at (x,y), using the specified
                    paint, transformed by the current matrix.
            */
            /*
                Now draw the bitmap to canvas.
                Bitmap will draw its center to circular bitmap center by keeping border spaces
            */
            canvas.DrawBitmap(bitmap, x, y, null);

            // Initializing a new Paint instance to draw circular border
            Paint borderPaint = new Paint();
            borderPaint.SetStyle(Paint.Style.Stroke);
            borderPaint.StrokeWidth = borderWidthHalf * 2;
            borderPaint.Color = Color.White;

            /*
                drawCircle(float cx, float cy, float radius, Paint paint)
                    Draw the specified circle using the specified paint.
            */
            /*
                Draw the circular border to bitmap.
                Draw the circle at the center of canvas.
             */
            canvas.DrawCircle(canvas.Width / 2, canvas.Height / 2, newBitmapSquareWidth / 2, borderPaint);

            /*
                RoundedBitmapDrawable
                    A Drawable that wraps a bitmap and can be drawn with rounded corners. You can create
                    a RoundedBitmapDrawable from a file path, an input stream, or from a Bitmap object.
            */
            /*
                public static RoundedBitmapDrawable create (Resources res, Bitmap bitmap)
                    Returns a new drawable by creating it from a bitmap, setting initial target density
                    based on the display metrics of the resources.
            */
            /*
                RoundedBitmapDrawableFactory
                    Constructs RoundedBitmapDrawable objects, either from Bitmaps directly, or from
                    streams and files.
            */
            // Create a new RoundedBitmapDrawable
            RoundedBitmapDrawable roundedBitmapDrawable = RoundedBitmapDrawableFactory.Create(Application.Context.Resources, roundedBitmap);

            /*
                setCornerRadius(float cornerRadius)
                    Sets the corner radius to be applied when drawing the bitmap.
            */
            // Set the corner radius of the bitmap drawable
            roundedBitmapDrawable.CornerRadius = bitmapRadius;

            /*
                setAntiAlias(boolean aa)
                    Enables or disables anti-aliasing for this drawable.
            */
            roundedBitmapDrawable.SetAntiAlias(true);

            // Return the RoundedBitmapDrawable
            return roundedBitmapDrawable;
        }

        private Bitmap GetImageBitmapFromUrl(string v)
        {
            Bitmap imageBitmap = null;
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(v);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}
