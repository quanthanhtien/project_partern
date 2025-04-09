namespace Favorites
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    // This is inspired by the way dopesheet icons and previews are handled
    // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Animation/AnimationWindow/DopeSheetEditor.cs
    public class PrivateAssetPreview : IDisposable
    {
        private int assetPreviewManagerID;
        private bool disposed;
        private IAssetPreviewPrivateMethods assetPreviewPrivate;

        public PrivateAssetPreview( int cacheId, IAssetPreviewPrivateMethods assetPreviewPrivate )
        {
            if( cacheId == 0 )
            {
                // Note(rafa): this exception is important, id 0 is used for the default preview cache
                // and we are trying to avoid using it so we are not affected by how the client code uses the
                // default cache.
                throw new ArgumentOutOfRangeException("cacheId");
            }

            if( assetPreviewPrivate == null)
                throw new ArgumentNullException("assetPreviewPrivate");

            this.assetPreviewPrivate  = assetPreviewPrivate;
            this.assetPreviewManagerID = cacheId;
        }

        public Texture2D GetAssetPreview(UnityEngine.Object reference)
        {
            if( disposed )
                throw new ObjectDisposedException("previewTextureManager");

            return assetPreviewPrivate.GetAssetPreview(reference, assetPreviewManagerID);;
        }

        public void SetCacheSize(int size)
        {
            if( disposed )
                throw new ObjectDisposedException("previewTextureManager");

            assetPreviewPrivate.SetCacheSize(size, assetPreviewManagerID);
        }

        public void Dispose()
        {
            if( disposed )
                return;
            assetPreviewPrivate.DeleteCache(assetPreviewManagerID);
            this.disposed = true;
        }

        // GetAssetPreview
        // GetAssetPreview
        // IsLoadingAssetPreview
        // IsLoadingAssetPreviews
        // HasAnyNewPreviewTexturesAvailable
        // HasAnyNewPreviewTexturesAvailable
        // SetPreviewTextureCacheSize
        // ClearTemporaryAssetPreviews
        // DeletePreviewTextureManagerByID
        // GetMiniTypeThumbnail
        // GetMiniTypeThumbnailFromClassID

    }

    public interface IAssetPreviewPrivateMethods
    {
        Texture2D GetAssetPreview(UnityEngine.Object asset, int cacheId);
        void SetCacheSize(int size, int cacheId);
        void DeleteCache( int cacheId);
    }

    public class ByReflectionAssetPreviewPrivateMethods : IAssetPreviewPrivateMethods
    {
        private MethodInfo getAssetPreviewInternal;
        private MethodInfo setPreviewTextureCacheSizeInternal;
        private MethodInfo deletePreviewTextureManagerByID;
        private IAssetPreviewPrivateMethods failSafeImplementation;
        private bool forceFail;
        private bool captureFailed;

        public ByReflectionAssetPreviewPrivateMethods(
            IAssetPreviewPrivateMethods failSafeImplementation)
            :this(failSafeImplementation, false)
        {    
        }

        public ByReflectionAssetPreviewPrivateMethods(
            IAssetPreviewPrivateMethods failSafeImplementation,
            bool forceFail)
        {
            this.failSafeImplementation = failSafeImplementation;
            this.forceFail = forceFail;
            CaptureInternalMethods();
        }

        private void CaptureInternalMethods()
        {
            if( forceFail == false )
            {
                // params are ( instanceID, cacheID )
                getAssetPreviewInternal = typeof(AssetPreview).GetMethod(
                    "GetAssetPreview", 
                    BindingFlags.NonPublic|BindingFlags.Static,
                    null, 
                    new Type[]{typeof(int), typeof(int)},
                    null);

                // params are (size, cacheID)
                setPreviewTextureCacheSizeInternal = typeof(AssetPreview).GetMethod(
                    "SetPreviewTextureCacheSize", 
                    BindingFlags.NonPublic|BindingFlags.Static,
                    null, 
                    new Type[]{typeof(int), typeof(int)},
                    null);

                // params are (cacheID)
                deletePreviewTextureManagerByID = typeof(AssetPreview).GetMethod(
                    "DeletePreviewTextureManagerByID", 
                    BindingFlags.NonPublic|BindingFlags.Static,
                    null, 
                    new Type[]{typeof(int)},
                    null);
            }
            
            if(getAssetPreviewInternal == null ||
               setPreviewTextureCacheSizeInternal == null ||
               deletePreviewTextureManagerByID == null )
            {
                captureFailed = true;
            }
        }

        public void DeleteCache(int cacheId)
        {
            if(captureFailed)
                failSafeImplementation.DeleteCache(cacheId);
            else
                deletePreviewTextureManagerByID.Invoke(null, new object[]{cacheId});
        }

        public Texture2D GetAssetPreview(UnityEngine.Object asset, int cacheId)
        {
            if(asset == null)
                return null;

            if ( captureFailed )
                return failSafeImplementation.GetAssetPreview(asset, cacheId);
            else
                return (Texture2D) getAssetPreviewInternal.Invoke(null, new object[]
                {
                    asset.GetInstanceID(), 
                    cacheId
                });
        }

        public void SetCacheSize(int size, int cacheId)
        {
            if(captureFailed)
                failSafeImplementation.SetCacheSize(size, cacheId);
            else
                setPreviewTextureCacheSizeInternal.Invoke(null, new object[]
                {
                    size, 
                    cacheId 
                });
        }
    }

    public class FailSafeAssetPreviewPrivateMethods : IAssetPreviewPrivateMethods
    {
        // (id, cacheSize)
        private readonly Dictionary<int, int> cacheSizes = new Dictionary<int, int>();
        private readonly int baseSize;

        public FailSafeAssetPreviewPrivateMethods(int baseSize)
        {
            this.baseSize = baseSize;
            RefreshCacheSize();
        }

        public int TotalCacheSize { get; private set; }

        public void DeleteCache(int cacheId)
        {
            cacheSizes.Remove(cacheId);
            RefreshCacheSize();
        }

        public Texture2D GetAssetPreview(UnityEngine.Object asset, int cacheId)
        {
            return AssetPreview.GetAssetPreview(asset);
        }

        public void SetCacheSize(int size, int cacheId)
        {
            cacheSizes[cacheId] = size;
            RefreshCacheSize();
        }

        private void RefreshCacheSize()
        {
            TotalCacheSize = baseSize + cacheSizes.Select(kvp => kvp.Value).Sum();
            AssetPreview.SetPreviewTextureCacheSize(TotalCacheSize);
        }
    }
}
