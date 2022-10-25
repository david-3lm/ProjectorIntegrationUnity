﻿using OpenCvSharp.Util;
using System;
using System.Collections.Generic;

namespace OpenCvSharp
{
#if LANG_JP
    /// <summary>
    /// 
    /// </summary>
#else
    /// <summary>
    /// 
    /// </summary>
#endif
    public class Subdiv2D : DisposableCvObject
    {
        /// <summary>
        /// Track whether Dispose has been called
        /// </summary>
        private bool disposed = false;

        #region Init and Disposal
        #region Constructor
#if LANG_JP
        /// <summary>
        /// デフォルトのパラメータで初期化.
        /// 実際は numberOfDisparities だけは指定する必要がある.
        /// </summary>
#else
        /// <summary>
        /// Default constructor
        /// </summary>
#endif
        public Subdiv2D()
        {
            ptr = NativeMethods.imgproc_Subdiv2D_new();
            if (ptr == IntPtr.Zero)
                throw new OpenCvSharpException();
        }

#if LANG_JP
        /// <summary>
        /// Subdiv2D コンストラクタ
        /// </summary>
        /// <param name="rect"></param>
#else
        /// <summary>
        /// Subdiv2D Constructor
        /// </summary>
        /// <param name="rect"></param>
#endif
        public Subdiv2D(Rect rect)
        {
            ptr = NativeMethods.imgproc_Subdiv2D_new(rect);
            if (ptr == IntPtr.Zero)
                throw new OpenCvSharpException();
        }
        #endregion
        #region Dispose
#if LANG_JP
        /// <summary>
        /// リソースの解放
        /// </summary>
#else
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
#endif
        public void Release()
        {
            Dispose(true);
        }
#if LANG_JP
        /// <summary>
        /// リソースの解放
        /// </summary>
        /// <param name="disposing">
        /// trueの場合は、このメソッドがユーザコードから直接が呼ばれたことを示す。マネージ・アンマネージ双方のリソースが解放される。
        /// falseの場合は、このメソッドはランタイムからファイナライザによって呼ばれ、もうほかのオブジェクトから参照されていないことを示す。アンマネージリソースのみ解放される。
        ///</param>
#else
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// If disposing equals true, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
        /// If false, the method has been called by the runtime from inside the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
        /// </param>
#endif
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    if (disposing)
                    {
                    }
                    if (ptr != IntPtr.Zero)
                    {
                        NativeMethods.imgproc_Subdiv2D_delete(ptr);
                    }
                    ptr = IntPtr.Zero;
                    disposed = true;
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }
        #endregion
        #endregion

        #region Constants
        /// <summary>
        /// 
        /// </summary>
        public const int
            PTLOC_ERROR = -2,
            PTLOC_OUTSIDE_RECT = -1,
            PTLOC_INSIDE = 0,
            PTLOC_VERTEX = 1,
            PTLOC_ON_EDGE = 2;
        /// <summary>
        /// 
        /// </summary>
        public const int
            NEXT_AROUND_ORG = 0x00,
            NEXT_AROUND_DST = 0x22,
            PREV_AROUND_ORG = 0x11,
            PREV_AROUND_DST = 0x33,
            NEXT_AROUND_LEFT = 0x13,
            NEXT_AROUND_RIGHT = 0x31,
            PREV_AROUND_LEFT = 0x20,
            PREV_AROUND_RIGHT = 0x02;
        #endregion

        #region Methods
        #region InitDelaunay
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        public void InitDelaunay(Rect rect)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            NativeMethods.imgproc_Subdiv2D_initDelaunay(ptr, rect);
        }
        #endregion
        #region Insert
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public int Insert(Point2f pt)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            return NativeMethods.imgproc_Subdiv2D_insert(ptr, pt);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptvec"></param>
        public void Insert(Point2f[] ptvec)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            if (ptvec == null)
                throw new ArgumentNullException("nameof(ptvec)");
            NativeMethods.imgproc_Subdiv2D_insert(ptr, ptvec, ptvec.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptvec"></param>
        public void Insert(IEnumerable<Point2f> ptvec)
        {
            if (ptvec == null)
                throw new ArgumentNullException("nameof(ptvec)");
            Insert(new List<Point2f>(ptvec).ToArray());
        }
        #endregion
        #region Locate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="edge"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public int Locate(Point2f pt, out int edge, out int vertex)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            return NativeMethods.imgproc_Subdiv2D_locate(ptr, pt, out edge, out vertex);
        }
        #endregion
        #region FindNearest
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public int FindNearest(Point2f pt)
        {
            Point2f nearestPt;
            return FindNearest(pt, out nearestPt);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="nearestPt"></param>
        /// <returns></returns>
        public int FindNearest(Point2f pt, out Point2f nearestPt)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            return NativeMethods.imgproc_Subdiv2D_findNearest(ptr, pt, out nearestPt);
        }
        #endregion
        #region GetEdgeList
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vec4f[] GetEdgeList()
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            IntPtr p;
            NativeMethods.imgproc_Subdiv2D_getEdgeList(ptr, out p);
            using (VectorOfVec4f vec = new VectorOfVec4f(p))
            {
                return vec.ToArray();
            }
        }
        #endregion
        #region GetTriangleList
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vec6f[] GetTriangleList()
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            IntPtr p;
            NativeMethods.imgproc_Subdiv2D_getTriangleList(ptr, out p);
            using (VectorOfVec6f vec = new VectorOfVec6f(p))
            {
                return vec.ToArray();
            }
        }
        #endregion
        #region GetVoronoiFacetList
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="facetList"></param>
        /// <param name="facetCenters"></param>
        public void GetVoronoiFacetList(IEnumerable<int> idx, out Point2f[][] facetList, out Point2f[] facetCenters)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");

            IntPtr facetListPtr, facetCentersPtr;
            if (idx == null)
            {
                NativeMethods.imgproc_Subdiv2D_getVoronoiFacetList(ptr, IntPtr.Zero, 0, out facetListPtr, out facetCentersPtr);
            }
            else
            {
                int[] idxArray = EnumerableEx.ToArray(idx);
                NativeMethods.imgproc_Subdiv2D_getVoronoiFacetList(ptr, idxArray, idxArray.Length, out facetListPtr, out facetCentersPtr);
            }

            using (VectorOfVectorPoint2f facetListVec = new VectorOfVectorPoint2f(facetListPtr))
            {
                facetList = facetListVec.ToArray();
            }
            using (VectorOfPoint2f facetCentersVec = new VectorOfPoint2f(facetCentersPtr))
            {
                facetCenters = facetCentersVec.ToArray();
            }
        }
        #endregion
        #region GetVertex
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public Point2f GetVertex(int vertex)
        {
            int firstEdge;
            return GetVertex(vertex, out firstEdge);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="firstEdge"></param>
        /// <returns></returns>
        public Point2f GetVertex(int vertex, out int firstEdge)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            return NativeMethods.imgproc_Subdiv2D_getVertex(ptr, vertex, out firstEdge);
        }
        #endregion
        #region GetEdge
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="nextEdgeType"></param>
        /// <returns></returns>
        public int GetEdge(int edge, int nextEdgeType)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            return NativeMethods.imgproc_Subdiv2D_getEdge(ptr, edge, nextEdgeType);
        }
        #endregion
        #region NextEdge
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public int NextEdge(int edge)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            return NativeMethods.imgproc_Subdiv2D_nextEdge(ptr, edge);
        }
        #endregion
        #region RotateEdge
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="rotate"></param>
        /// <returns></returns>
        public int RotateEdge(int edge, int rotate)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            return NativeMethods.imgproc_Subdiv2D_rotateEdge(ptr, edge, rotate);
        }
        #endregion
        #region SymEdge
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public int SymEdge(int edge)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            return NativeMethods.imgproc_Subdiv2D_symEdge(ptr, edge);
        }
        #endregion
        #region EdgeOrg
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public int EdgeOrg(int edge)
        {
            Point2f orgpt;
            return EdgeOrg(edge, out orgpt);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="orgpt"></param>
        /// <returns></returns>
        public int EdgeOrg(int edge, out Point2f orgpt)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            return NativeMethods.imgproc_Subdiv2D_edgeOrg(ptr, edge, out orgpt);
        }
        #endregion
        #region EdgeDst
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public int EdgeDst(int edge)
        {
            Point2f dstpt;
            return EdgeDst(edge, out dstpt);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="dstpt"></param>
        /// <returns></returns>
        public int EdgeDst(int edge, out Point2f dstpt)
        {
            if (disposed)
                throw new ObjectDisposedException("Subdiv2D", "");
            return NativeMethods.imgproc_Subdiv2D_edgeDst(ptr, edge, out dstpt);
        }
        #endregion
        #endregion
    }
}
