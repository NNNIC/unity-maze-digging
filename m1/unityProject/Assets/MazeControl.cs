using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MazeControl : MonoBehaviour {
 
    #region manager
    Action<bool> m_curfunc;
    Action<bool> m_nextfunc;

    bool         m_noWait;
    
    void _update()
    {
        while(true)
        {
            var bFirst = false;
            if (m_nextfunc!=null)
            {
                m_curfunc = m_nextfunc;
                m_nextfunc = null;
                bFirst = true;
            }
            m_noWait = false;
            if (m_curfunc!=null)
            {   
                m_curfunc(bFirst);
            }
            if (!m_noWait) break;
        }
    }
    void Goto(Action<bool> func)
    {
        m_nextfunc = func;
    }
    bool CheckState(Action<bool> func)
    {
        return m_curfunc == func;
    }
    bool HasNextState()
    {
        return m_nextfunc != null;
    }
    void NoWait()
    {
        m_noWait = true;
    }
    #endregion
    #region gosub
    List<Action<bool>> m_callstack = new List<Action<bool>>();
    void GoSubState(Action<bool> nextstate, Action<bool> returnstate)
    {
        m_callstack.Insert(0,returnstate);
        Goto(nextstate);
    }
    void ReturnState()
    {
        var nextstate = m_callstack[0];
        m_callstack.RemoveAt(0);
        Goto(nextstate);
    }
    #endregion 

    void _start()
    {
        Goto(S_START);
    }
    public bool IsEnd()     
    { 
        return CheckState(S_END); 
    }

	#region    // [PSGG OUTPUT START] indent(4) $/./$
    //             psggConverterLib.dll converted from psgg-file:MazeControl.psgg

    /*
        E_0000
    */
    public GameObject   m_root;
    public const int m_width  = 41;
    public const int m_height = 41;
    GameObject[,] m_cubelist;
    List<int[]> m_pathlist = new List<int[]>();
    /*
        E_0001
    */
    public enum DIR
    {
        none,
        UP,
        RIGHT,
        DOWN,
        LEFT
    }
    List<DIR> m_availableDirs = new List<DIR>();
    /*
        S_AVAILABLE_DIRECTIONS
        移動可能な方向を収集する。
    */
    void S_AVAILABLE_DIRECTIONS(bool bFirst)
    {
        if (bFirst)
        {
            m_availableDirs = collect_available_dirs(m_cur_x,m_cur_y);
        }
        // branch
        if (m_availableDirs.Count > 0) { Goto( S_DIGDIR ); }
        else { Goto( S_FIND_NEW_START ); }
    }
    /*
        S_CreateCubes
    */
    void S_CreateCubes(bool bFirst)
    {
        if (bFirst)
        {
            m_cubelist = new GameObject[m_width, m_height];
            for(var x = 0; x < m_width; x++)
            {
                for(var y = 0; y < m_height; y++)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = m_root.transform;
                    cube.transform.localPosition = new Vector3( - ((float)m_width * 0.5f )  + x,0, -((float)m_height * 0.5f) + y);
                    m_cubelist[x,y] = cube;
                }
            }
        }
        //
        if (!HasNextState())
        {
            Goto(S_DEL_OUTER);
        }
    }
    /*
        S_DEL_OUTER
        外枠を消す
    */
    void S_DEL_OUTER(bool bFirst)
    {
        if (bFirst)
        {
            var left_x  = 0;
            var right_x = m_width - 1;
            var near_y  = 0;
            var far_y   = m_height - 1;
            for(var y = 0; y < m_height; y++) {
                m_cubelist[left_x,y].SetActive(false);
                m_cubelist[right_x,y].SetActive(false);
            }
            for(var x = 0; x < m_width; x++)
            {
                m_cubelist[x, near_y].SetActive(false);
                m_cubelist[x, far_y].SetActive(false);
            }
        }
        //
        if (!HasNextState())
        {
            Goto(S_STARTPOINT);
        }
    }
    /*
        S_DIGDIR
        可能な方向の一つを選び
        ２つ掘る。
        記録する。
    */
    void S_DIGDIR(bool bFirst)
    {
        if (bFirst)
        {
            var d = RandomUtil.Get(m_availableDirs);
            if (d == DIR.UP)    { dig( 0, 1); dig( 0, 2,true); }
            if (d == DIR.RIGHT) { dig( 1, 0); dig( 2, 0,true); }
            if (d == DIR.DOWN)  { dig( 0,-1); dig( 0,-2,true); }
            if (d == DIR.LEFT)  { dig(-1, 0); dig(-2, 0,true); }
        }
        //
        if (!HasNextState())
        {
            Goto(S_AVAILABLE_DIRECTIONS);
        }
    }
    /*
        S_DIGSP
        開始位置を掘る
        記録する
    */
    void S_DIGSP(bool bFirst)
    {
        if (bFirst)
        {
            dig(0,0, true);
        }
        //
        if (!HasNextState())
        {
            Goto(S_AVAILABLE_DIRECTIONS);
        }
    }
    /*
        S_END
    */
    void S_END(bool bFirst)
    {
    }
    /*
        S_FIND_NEW_START
    */
    void S_FIND_NEW_START(bool bFirst)
    {
        var b = find_new_start();
        // branch
        if (b) { Goto( S_AVAILABLE_DIRECTIONS ); }
        else { Goto( S_SET_OUTER ); }
    }
    /*
        S_SET_OUTER
        外枠復活
    */
    void S_SET_OUTER(bool bFirst)
    {
        if (bFirst)
        {
            var left_x  = 0;
            var right_x = m_width - 1;
            var near_y  = 0;
            var far_y   = m_height - 1;
            for(var y = 0; y < m_height; y++) {
                m_cubelist[left_x,y].SetActive(true);
                m_cubelist[right_x,y].SetActive(true);
            }
            for(var x = 0; x < m_width; x++)
            {
                m_cubelist[x, near_y].SetActive(true);
                m_cubelist[x, far_y].SetActive(true);
            }
        }
        //
        if (!HasNextState())
        {
            Goto(S_END);
        }
    }
    /*
        S_START
    */
    void S_START(bool bFirst)
    {
        Goto(S_CreateCubes);
        NoWait();
    }
    /*
        S_STARTPOINT
    */
    public int m_cur_x;
    public int m_cur_y;
    void S_STARTPOINT(bool bFirst)
    {
        if (bFirst)
        {
            m_cur_x = rand_odd(1,m_width-1);
            m_cur_y = rand_odd(1,m_height-1);
        }
        //
        if (!HasNextState())
        {
            Goto(S_DIGSP);
        }
    }


	#endregion // [PSGG OUTPUT END]

	// write your code below


    #region Monobehaviour framework
    void Start()
    {
        _start();
    }
    void Update()
    {
        if (!IsEnd()) 
        {
            _update();
        }
    }
    #endregion


    int rand_odd(int start, int end)
    {
        for(var loop = 0; loop<1000; loop++)
        {
            var range = end - start;
            var a = start + RandomUtil.GetInt(range);
            if (a % 2 == 1) return a;
        }
        return start;
    }

    bool IsWall(int ix, int iy)
    {
        var x = m_cur_x + ix;
        var y = m_cur_y + iy;
        if (x < 0 || x > m_width-1 ) return false;
        if (y < 0 || y > m_height-1) return false;
        return m_cubelist[x,y].activeSelf;
    }

    void dig(int ix, int iy, bool bSetCur=false)
    {
        var x = m_cur_x + ix;
        var y = m_cur_y + iy;

        if (x < 0 || x > m_width-1 ) 
            throw new SystemException("{016DDC74-77F1-4D6B-B20E-D4527B59AAA4}");
        if (y < 0 || y > m_height-1) throw new SystemException("{F243654C-BAC1-4347-B346-FEEA2D609320}");
        if (!m_cubelist[x,y].activeSelf) throw new SystemException("{48AA7FBF-5814-4BCD-9B0D-8683D0EE630D}");
        m_cubelist[x,y].SetActive(false);
        if (x%2==1 && y%2==1) //奇数だけリスト
        {
            m_pathlist.Add(new int[] {x,y});
        }
        if (bSetCur)
        {
            m_cur_x = x;
            m_cur_y = y;
        }
    }

    List<DIR> collect_available_dirs(int cx , int cy)
    {
        Func<int,int,bool> is_wall=(ix, iy)=>
        {
            var x = cx + ix;
            var y = cy + iy;
            if (x < 0 || x > m_width-1 ) return false;
            if (y < 0 || y > m_height-1) return false;
            return m_cubelist[x,y].activeSelf;
        };
        var list = new List<DIR>();
        //Up
        if (is_wall( 0, 1) && is_wall( 0, 2)) list.Add(DIR.UP);
        //Right
        if (is_wall( 1, 0) && is_wall( 2, 0)) list.Add(DIR.RIGHT);
        //Down
        if (is_wall( 0,-1) && is_wall( 0,-2)) list.Add(DIR.DOWN);
        //Left
        if (is_wall(-1, 0) && is_wall(-2, 0)) list.Add(DIR.LEFT);

        return list;
    }

    bool find_new_start()
    {
        var nocheckpathlist = new List<int>();
        for(var i = 0; i < m_pathlist.Count; i++) nocheckpathlist.Add(i);

        while(nocheckpathlist.Count>0)
        {
            var index      = RandomUtil.Get(nocheckpathlist);
            nocheckpathlist.Remove(index);

            var cand_point = m_pathlist[index];
            if (cand_point.Length!=2) throw new SystemException("{2F1EDAA0-C514-4505-A374-BC45BAE5AD76}");
            var cx = cand_point[0];
            var cy = cand_point[1];

            var cdirs = collect_available_dirs(cx,cy);
            if (cdirs.Count > 0)
            {
                m_cur_x = cx;
                m_cur_y = cy;
                return true;
            }
        }

        return false;
    }

}

/*  :::: PSGG MACRO ::::
:psgg-macro-start

commentline=// {%0}

@branch=@@@
<<<?"{%0}"/^brifc{0,1}$/
if ([[brcond:{%N}]]) { Goto( {%1} ); }
>>>
<<<?"{%0}"/^brelseifc{0,1}$/
else if ([[brcond:{%N}]]) { Goto( {%1} ); }
>>>
<<<?"{%0}"/^brelse$/
else { Goto( {%1} ); }
>>>
<<<?"{%0}"/^br_/
{%0}({%1});
>>>
@@@

:psgg-macro-end
*/

