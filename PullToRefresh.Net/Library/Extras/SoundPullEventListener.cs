/*******************************************************************************
 * Copyright 2011, 2012 Chris Banes.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/
//package com.handmark.pulltorefresh.library.extras;

//import java.util.HashMap;

//import android.content.Context;
//import android.media.MediaPlayer;
//import android.view.View;

using Android.Content;
using Android.Media;
//import com.handmark.pulltorefresh.library.PullToRefreshBase;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.Mode;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.State;
using Android.Views;
using Com.Handmark.PullToRefresh.Library;
using System.Collections.Generic;

using State = Com.Handmark.PullToRefresh.Library.State;

namespace Com.Handmark.PullToRefresh.Library.Extras
{
    public class SoundPullEventListener<T> : OnPullEventListener<T> where T:View
    {

        private readonly Context mContext;

        private readonly Dictionary<State, int> mSoundMap;

        private MediaPlayer mCurrentMediaPlayer;

        /**
         * Constructor
         * 
         * @param context - Context
         */
        public SoundPullEventListener(Context context)
        {
            mContext = context;
            mSoundMap = new Dictionary<State, int>();
        }

        

        /**
         * Set the Sounds to be played when a Pull Event happens. You specify which
         * sound plays for which events by calling this method multiple times for
         * each event.
         * <p/>
         * If you've already set a sound for a certain event, and add another sound
         * for that event, only the new sound will be played.
         * 
         * @param event - The event for which the sound will be played.
         * @param resId - Resource Id of the sound file to be played (e.g.
         *            <var>R.raw.pull_sound</var>)
         */
        public void addSoundEvent(State sevent, int resId)
        {
            mSoundMap.Add(sevent, resId);
        }

        /**
         * Clears all of the previously set sounds and events.
         */
        public void clearSounds()
        {
            mSoundMap.Clear();
        }

        /**
         * Gets the current (or last) MediaPlayer instance.
         */
        public MediaPlayer getCurrentMediaPlayer()
        {
            return mCurrentMediaPlayer;
        }

        private void playSound(int resId)
        {
            // Stop current player, if there's one playing
            if (null != mCurrentMediaPlayer)
            {
                mCurrentMediaPlayer.Stop();
                mCurrentMediaPlayer.Release();
            }

            mCurrentMediaPlayer = MediaPlayer.Create(mContext, resId);
            if (null != mCurrentMediaPlayer)
            {
                mCurrentMediaPlayer.Start();
            }
        }




        public void onPullEvent(PullToRefreshBase<T> refreshView, State state, PtrMode direction)
        {
            if (mSoundMap.ContainsKey(state))
            {
                int soundResIdObj = mSoundMap[state];
                if (null != soundResIdObj)
                {
                    playSound(soundResIdObj);
                }
            }
        }
    }
}