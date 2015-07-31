using Android.Runtime;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace Com.Handmark.PullToRefresh.Library
{
    public interface ILoadingLayout
    {
        /**
	 * Set the Last Updated Text. This displayed under the main label when
	 * Pulling
	 * 
	 * @param label - Label to set
	 */
        void setLastUpdatedLabel(string label);

        /**
         * Set the drawable used in the loading layout. This is the same as calling
         * <code>setLoadingDrawable(drawable, Mode.BOTH)</code>
         * 
         * @param drawable - Drawable to display
         */
        void setLoadingDrawable(Drawable drawable);

        /**
         * Set Text to show when the Widget is being Pulled
         * <code>setPullLabel(releaseLabel, Mode.BOTH)</code>
         * 
         * @param pullLabel - CharSequence to display
         */
        void setPullLabel(string pullLabel);

        /**
         * Set Text to show when the Widget is refreshing
         * <code>setRefreshingLabel(releaseLabel, Mode.BOTH)</code>
         * 
         * @param refreshingLabel - CharSequence to display
         */
        void setRefreshingLabel(string refreshingLabel);

        /**
         * Set Text to show when the Widget is being pulled, and will refresh when
         * released. This is the same as calling
         * <code>setReleaseLabel(releaseLabel, Mode.BOTH)</code>
         * 
         * @param releaseLabel - CharSequence to display
         */
        void setReleaseLabel(string releaseLabel);

        /**
         * Set's the Sets the typeface and style in which the text should be
         * displayed. Please see
         * {@link android.widget.TextView#setTypeface(Typeface)
         * TextView#setTypeface(Typeface)}.
         */
        void setTextTypeface(Typeface tf);
    }
}