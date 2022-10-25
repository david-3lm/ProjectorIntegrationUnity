﻿using OpenCvSharp.Util;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCvSharp
{
    /// <summary>
    /// 
    /// </summary>
    internal class VectorOfKeyPoint : DisposableCvObject, IStdVector<KeyPoint>
    {
        /// <summary>
        /// Track whether Dispose has been called
        /// </summary>
        private bool disposed = false;

        #region Init and Dispose

        /// <summary>
        /// 
        /// </summary>
        public VectorOfKeyPoint()
        {
            ptr = NativeMethods.vector_KeyPoint_new1();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        public VectorOfKeyPoint(IntPtr ptr)
        {
            this.ptr = ptr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public VectorOfKeyPoint(int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("nameof(size)");
            ptr = NativeMethods.vector_KeyPoint_new2(new IntPtr(size));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public VectorOfKeyPoint(IEnumerable<KeyPoint> data)
        {
            if (data == null)
                throw new ArgumentNullException("nameof(data)");
            KeyPoint[] array = EnumerableEx.ToArray(data);
            ptr = NativeMethods.vector_KeyPoint_new3(array, new IntPtr(array.Length));
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// If disposing equals true, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
        /// If false, the method has been called by the runtime from inside the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    if (IsEnabledDispose)
                    {
                        NativeMethods.vector_KeyPoint_delete(ptr);
                    }
                    disposed = true;
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// vector.size()
        /// </summary>
        public int Size
        {
            get { return NativeMethods.vector_KeyPoint_getSize(ptr).ToInt32(); }
        }

        /// <summary>
        /// &amp;vector[0]
        /// </summary>
        public IntPtr ElemPtr
        {
            get { return NativeMethods.vector_KeyPoint_getPointer(ptr); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts std::vector to managed array
        /// </summary>
        /// <returns></returns>
        public KeyPoint[] ToArray()
        {
            int size = Size;
            if (size == 0)
            {
                return new KeyPoint[0];
            }
            KeyPoint[] dst = new KeyPoint[size];
            using (var dstPtr = new ArrayAddress1<KeyPoint>(dst))
            {
                Util.Utility.CopyMemory(dstPtr, ElemPtr, Marshal.SizeOf(typeof(KeyPoint)) * dst.Length);
            }
            return dst;
        }

        #endregion
    }
}
