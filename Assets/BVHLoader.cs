// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// 姓名：谢悦
// 学号：1900013055
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ! 在以下部分定义需要的类或结构体
public class Node{
    public int father;//该节点爸爸的标号
    public int id;//该节点的标号
    public string name;//该节点的名字
    public Vector3 offset;//该节点的offset数据
    public List<List<Quaternion>> rotationRow;//motion数据处理出来的每一帧的旋转矩阵
    public Vector3[] axisOrder;
}


public class BVHLoader : MonoBehaviour
{
    private string bvh_fname1 = "Assets\\BVH\\static.bvh";
    private string bvh_fname2 = "Assets\\BVH\\Walking.bvh";
    private string bvh_fname3 = "Assets\\BVH\\BackwardsWalk.bvh";
    private string bvh_fname4 = "Assets\\BVH\\SlowTrot.bvh";
    private string bvh_fname5 = "Assets\\BVH\\Body.bvh";//
    private string bvh_fname6 = "Assets\\BVH\\JumpRope.bvh";// for a show
    private string bvh_fname7 = "Assets\\BVH\\Obstacle.bvh";
    private string bvh_fname8 = "Assets\\BVH\\static8.bvh";
    private string bvh_fname00 = "Assets\\BVH\\up.bvh";
    private string bvh_fname01 = "Assets\\BVH\\down.bvh";
    private string bvh_fname02 = "Assets\\BVH\\left.bvh";//
    private string bvh_fname03 = "Assets\\BVH\\right.bvh";// for a show

    private List<string> joints = new List<string>();
    // game object列表
    private List<GameObject> gameObjects = new List<GameObject>();
    // 时间戳
    private int time_step = 0;
    // bvh的帧数
    private List<int> frame_num = new List<int>();
    //当前动作
    private int now_motion = 0;

    //是否在lerp阶段，如果不是，为-1，是的0-10
    private int is_lerp = -1;
    //插值的上一个动作是编号和帧数
    private int from_motion = 0;
    private int from_time = 0;
    //lerp的时间
    private int during_time = 15;
    //当前动作初始点的朝向
    private Quaternion  org_rot = new Quaternion(0.0f,0.0f,0.0f,1.0f);
    //当前动作初始点所记录的hip的旋转
    private Quaternion  last_rot = new Quaternion(0.0f,0.0f,0.0f,1.0f);
    //当前动作初始点的位置
    private Vector3 org_pos = new Vector3(0.0f,0.0f,0.0f);

    // ! 在这里声明你需要的其他数据结构
    private List<Node> treeNode = new List<Node>();
    private Stack<int> deal_data = new Stack<int>();
    private int fathername = 0;
    private Vector3 offset_tmp = new Vector3();
    private Node tmpData = new Node();
    private int channel_num = 0;
    private int Mx=0,My=0,Mz=0;
    private int Node_num = 0;//结点计数，用于标记父节点的编号

    private List<List<Vector3>> positions_motion = new List<List<Vector3>>();
    // ! 在以上空白写下你的代码
    
    
    // Start is called before the first frame update
    void Start()
    {
        deal_data.Push(-1);
        Application.targetFrameRate = 60;
        StreamReader bvh_file1 = new StreamReader(new FileStream(bvh_fname1, FileMode.Open));
        StreamReader bvh_file2 = new StreamReader(new FileStream(bvh_fname2, FileMode.Open));
        StreamReader bvh_file3 = new StreamReader(new FileStream(bvh_fname3, FileMode.Open));
        StreamReader bvh_file4 = new StreamReader(new FileStream(bvh_fname4, FileMode.Open));
        StreamReader bvh_file5 = new StreamReader(new FileStream(bvh_fname5, FileMode.Open));
        StreamReader bvh_file6 = new StreamReader(new FileStream(bvh_fname6, FileMode.Open));
        StreamReader bvh_file7 = new StreamReader(new FileStream(bvh_fname7, FileMode.Open));
        StreamReader bvh_file8 = new StreamReader(new FileStream(bvh_fname8, FileMode.Open));
        StreamReader bvh_file9 = new StreamReader(new FileStream(bvh_fname00, FileMode.Open));
        StreamReader bvh_file10 = new StreamReader(new FileStream(bvh_fname01, FileMode.Open));
        StreamReader bvh_file11 = new StreamReader(new FileStream(bvh_fname02, FileMode.Open));
        StreamReader bvh_file12 = new StreamReader(new FileStream(bvh_fname03, FileMode.Open));
        // 以下读入bvh中的骨骼数据
        while(!bvh_file1.EndOfStream){
            string line = bvh_file1.ReadLine();
            string line2 = bvh_file2.ReadLine();
            string line3 = bvh_file3.ReadLine();
            string line4 = bvh_file4.ReadLine();
            string line5 = bvh_file5.ReadLine();
            string line6 = bvh_file6.ReadLine();
            string line7 = bvh_file7.ReadLine();
            string line8 = bvh_file8.ReadLine();
            string line9 = bvh_file9.ReadLine();
            string line10 = bvh_file10.ReadLine();
            string line11 = bvh_file11.ReadLine();
            string line12 = bvh_file12.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            string str2 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line2, " ");
            string[] split_line2 = str2.Split(' ');
            string str3 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line3, " ");
            string[] split_line3 = str3.Split(' ');
            string str4 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line4, " ");
            string[] split_line4 = str4.Split(' ');
            string str5 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line5, " ");
            string[] split_line5 = str5.Split(' ');
            string str6 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line6, " ");
            string[] split_line6 = str6.Split(' ');
            string str7 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line7, " ");
            string[] split_line7 = str7.Split(' ');
            string str8 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line8, " ");
            string[] split_line8 = str8.Split(' ');
            string str9 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line9, " ");
            string[] split_line9 = str9.Split(' ');
            string str10 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line10, " ");
            string[] split_line10 = str10.Split(' ');
            string str11 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line11, " ");
            string[] split_line11 = str11.Split(' ');
            string str12 = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line12, " ");
            string[] split_line12 = str12.Split(' ');
            // 处理bvh文件中的character hierarchy部分
            if (line.Contains("ROOT") || line.Contains("JOINT")){
                joints.Add(split_line[split_line.Length-1]);
                deal_data.Push(Node_num);
                tmpData = new Node();
                tmpData.name = split_line[split_line.Length-1];
                tmpData.id = Node_num;
                tmpData.father = fathername;
            } 
            else if (line.Contains("End Site")){
                joints.Add(treeNode[fathername].name+"_end");
                //print(joints.Count);
                deal_data.Push(Node_num);
                tmpData = new Node();
                tmpData.name = treeNode[fathername].name+"_end";
                tmpData.id = Node_num;
                tmpData.father = fathername;
            }
            else if (line.Contains("{")){
                fathername = deal_data.Peek();
            }
            else if (line.Contains("}")){//将结点从堆中弹出
                deal_data.Pop();
                fathername = deal_data.Peek();
            }
            else if (line.Contains("OFFSET")){
                int j = 0;
                for (j = 0; j < split_line.Length; j++){
                    if (split_line[j] == "OFFSET")break;
                }
                float x = (float)System.Convert.ToDouble(split_line[j+1])/50;
                float y = (float)System.Convert.ToDouble(split_line[j+2])/50;
                float z = (float)System.Convert.ToDouble(split_line[j+3])/50;
                offset_tmp = new Vector3(x, y, z);
                tmpData.offset = offset_tmp;
                if(tmpData.name.Contains("_end")){
                    treeNode.Add(tmpData);
                    Node_num++;
                }
            }
            else if (line.Contains("CHANNELS")){
                channel_num = split_line.Length;
                for(int i=0;i<split_line.Length;i++){
                    if(split_line[i].Contains("Xrotation"))Mx=i-channel_num+3;
                    else if(split_line[i].Contains("Yrotation"))My=i-channel_num+3;
                    else if(split_line[i].Contains("Zrotation"))Mz=i-channel_num+3;
                }
                Vector3[] tmpVec = new Vector3[3];
                tmpVec[Mx]=Vector3.right;
                tmpVec[My]=Vector3.up;
                tmpVec[Mz]=Vector3.forward;
                tmpData.axisOrder = tmpVec;
                tmpData.rotationRow = new List<List<Quaternion>>();
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                tmpData.rotationRow.Add(new List<Quaternion>());
                treeNode.Add(tmpData);
                Node_num++;
            }
            else if (line.Contains("Frame Time")){
                // Frame Time是数据部分前的最后一行，读到这一行后跳出循环
                break;
            }
            else if (line.Contains("Frames:")){
                // 获取帧数
                frame_num.Add(int.Parse(split_line[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line2[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line3[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line4[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line5[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line6[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line7[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line8[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line9[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line10[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line11[split_line.Length - 1]));
                frame_num.Add(int.Parse(split_line12[split_line.Length - 1]));
            }
        }
        // 接下来处理bvh文件中的动作数据部分 为了省事直接copy8遍了x
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        positions_motion.Add(new List<Vector3>());
        while(!bvh_file1.EndOfStream){
            string line = bvh_file1.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[0].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[0].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        while(!bvh_file2.EndOfStream){
            string line = bvh_file2.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[1].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[1].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        while(!bvh_file3.EndOfStream){
            string line = bvh_file3.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[2].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[2].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        while(!bvh_file4.EndOfStream){
            string line = bvh_file4.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[3].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[3].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        while(!bvh_file5.EndOfStream){
            string line = bvh_file5.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[4].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[4].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        while(!bvh_file6.EndOfStream){
            string line = bvh_file6.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[5].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[5].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        while(!bvh_file7.EndOfStream){
            string line = bvh_file7.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[6].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[6].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        while(!bvh_file8.EndOfStream){
            string line = bvh_file8.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[7].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[7].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        
        while(!bvh_file9.EndOfStream){
            string line = bvh_file9.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[8].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[8].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        
        while(!bvh_file10.EndOfStream){
            string line = bvh_file10.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[9].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[9].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        
        while(!bvh_file11.EndOfStream){
            string line = bvh_file11.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[10].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[10].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        
        while(!bvh_file12.EndOfStream){
            string line = bvh_file12.ReadLine();
            string str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(line, " ");
            string[] split_line = str.Split(' ');
            split_line = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            float x = (float)System.Convert.ToDouble(split_line[0])/50;
            float y = (float)System.Convert.ToDouble(split_line[1])/50;
            float z = (float)System.Convert.ToDouble(split_line[2])/50;
            positions_motion[11].Add(new Vector3(x, y, z));
            int num = 0;
            for(int i=3;i<split_line.Length;i+=3){
                float[] rot = new float[3];
                float.TryParse(split_line[i],out rot[0]);
                float.TryParse(split_line[i+1],out rot[1]);
                float.TryParse(split_line[i+2],out rot[2]);
                Quaternion q = Quaternion.AngleAxis(rot[0],treeNode[num].axisOrder[0])*
                               Quaternion.AngleAxis(rot[1],treeNode[num].axisOrder[1])*
                               Quaternion.AngleAxis(rot[2],treeNode[num].axisOrder[2]);
                treeNode[num].rotationRow[11].Add(q);
                num++;
                if(num<treeNode.Count&&treeNode[num].name.Contains("_end"))num++;
            }
        }
        // 按关节名称获取所有的game object
        GameObject tmp_obj = new GameObject();
        for (int i = 0; i < joints.Count; i++){
            tmp_obj = GameObject.Find(joints[i]);
            gameObjects.Add(tmp_obj);
        }
        org_pos = new Vector3(0.0f,0.0f,0.0f);//new Vector3(positions_motion[now_motion][0].x,0.0f,positions_motion[now_motion][0].z);
        org_rot = Quaternion.Euler(new Vector3(0.0f,0.0f,0.0f));//treeNode[0].rotationRow[now_motion][0].eulerAngles.y,0.0f));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 joint_position = new Vector3(0.0F, 0.0F, 0.0F);
        Quaternion joint_orientation = Quaternion.identity;
        // ! 定义你需要的局部变量
        if(is_lerp!=-1){
            // 进行两个动作衔接之间的插值，插值过程中，保证人物朝向和位移不变
            for (int i = 0; i < joints.Count; i++){
                float  t = ((float)is_lerp)/(float)during_time;
                // ! 进行前向运动学的计算，根据之前解析出的每一帧局部位置、旋转获得每个关节的全局位置、旋转
                if(treeNode[i].name.Contains("_end")||treeNode[i].name.Contains("_End")){
                    joint_position = gameObjects[treeNode[i].father].transform.position+gameObjects[treeNode[i].father].transform.rotation*treeNode[i].offset;
                    joint_orientation = gameObjects[treeNode[i].father].transform.rotation;
                }
                else if(i==0){//对于hip
                    //其旋转和位移需要乘以当前点动作第一帧之前的初始朝向，
                    //位移用每一帧数据位置的x轴和z轴的值，y轴的值对两帧之间的高度进行插值。
                    Quaternion tmp_pos = Quaternion.Euler(new Vector3(0.0f,org_rot.eulerAngles.y-treeNode[i].rotationRow[now_motion][0].eulerAngles.y,0.0f));
                    Vector3 tmp = tmp_pos*(positions_motion[now_motion][time_step]-positions_motion[now_motion][0]);
                    float tmp_y = (1-t)*positions_motion[from_motion][from_time].y + t*positions_motion[now_motion][time_step].y;
                    
                    joint_position = org_pos + new Vector3(tmp.x,tmp_y,tmp.z);
                    Vector3 tmp_q = treeNode[i].rotationRow[now_motion][time_step].eulerAngles;
                    Vector3 tmp_qq = treeNode[i].rotationRow[now_motion][time_step].eulerAngles-treeNode[i].rotationRow[now_motion][0].eulerAngles;
                    if(tmp_qq.y<0.0f)tmp_qq.y+=360.0f;
                    else if(tmp_qq.y>360.0f)tmp_qq.y-=360.0f;
                    tmp_qq.y += org_rot.eulerAngles.y;
                    if(tmp_qq.y<0.0f)tmp_qq.y+=360.0f;
                    else if(tmp_qq.y>360.0f)tmp_qq.y-=360.0f;

                    joint_orientation = Quaternion.Euler(new Vector3(tmp_q.x,tmp_qq.y,tmp_q.z));
                    joint_orientation = Quaternion.Slerp(last_rot,joint_orientation,t);
                }
                else{
                    joint_position = gameObjects[treeNode[i].father].transform.position+gameObjects[treeNode[i].father].transform.rotation*treeNode[i].offset;
                    joint_orientation = gameObjects[treeNode[i].father].transform.rotation*
                                        Quaternion.Slerp(treeNode[i].rotationRow[from_motion][from_time],treeNode[i].rotationRow[now_motion][time_step],t);
                }
                gameObjects[i].transform.position = joint_position;
                gameObjects[i].transform.rotation = joint_orientation;
            }
            is_lerp += 1;
            if(is_lerp == during_time){//结束插值过渡段
                is_lerp = -1;
            }
        }
        else{
            // 一个动作中的FK解算
            for (int i = 0; i < joints.Count; i++){
                // ! 进行前向运动学的计算，根据之前解析出的每一帧局部位置、旋转获得每个关节的全局位置、旋转
                if(treeNode[i].name.Contains("_end")||treeNode[i].name.Contains("_End")){
                    joint_position = gameObjects[treeNode[i].father].transform.position+gameObjects[treeNode[i].father].transform.rotation*treeNode[i].offset;
                    joint_orientation = gameObjects[treeNode[i].father].transform.rotation;
                }
                else if(i==0){
                    //其旋转和位移需要乘以当前点动作第一帧之前的初始朝向，
                    Quaternion tmp_pos = Quaternion.Euler(new Vector3(0.0f,org_rot.eulerAngles.y-treeNode[i].rotationRow[now_motion][0].eulerAngles.y,0.0f));
                    Vector3 tmp = tmp_pos*(positions_motion[now_motion][time_step]-positions_motion[now_motion][0]);
                    joint_position = org_pos + new Vector3(tmp.x,positions_motion[now_motion][time_step].y,tmp.z);

                    Vector3 tmp_q = treeNode[i].rotationRow[now_motion][time_step].eulerAngles;
                    Vector3 tmp_qq = treeNode[i].rotationRow[now_motion][time_step].eulerAngles-treeNode[i].rotationRow[now_motion][0].eulerAngles;
                    if(tmp_qq.y<0.0f)tmp_qq.y+=360.0f;
                    else if(tmp_qq.y>360.0f)tmp_qq.y-=360.0f;
                    tmp_qq.y += org_rot.eulerAngles.y;
                    if(tmp_qq.y<0.0f)tmp_qq.y+=360.0f;
                    else if(tmp_qq.y>360.0f)tmp_qq.y-=360.0f;

                    joint_orientation = Quaternion.Euler(new Vector3(tmp_q.x,tmp_qq.y,tmp_q.z));
                }
                else{
                    joint_position = gameObjects[treeNode[i].father].transform.position+gameObjects[treeNode[i].father].transform.rotation*treeNode[i].offset;
                    joint_orientation = gameObjects[treeNode[i].father].transform.rotation*treeNode[i].rotationRow[now_motion][time_step];
                }
                gameObjects[i].transform.position = joint_position;
                gameObjects[i].transform.rotation = joint_orientation;
            }
            // 更新时间戳
            time_step += 1;
            //由于数据头尾不连贯，需要做一个头尾之间的微小插值
            if(time_step==frame_num[now_motion]-1&&now_motion<8){
                org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
                org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
                last_rot = gameObjects[0].transform.rotation;
                from_motion = now_motion;
                from_time = time_step;
                now_motion = now_motion;
                during_time = 15;//这里通过手动调整插值的帧数，进行速度上的调整
                time_step = 0;
                is_lerp = 1;
            }
            if(time_step==frame_num[now_motion]-1&&now_motion>=8){//如果是对四个方向移动进行调整，需要不改变其原始朝向
                org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
                // org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
                last_rot = gameObjects[0].transform.rotation;
                from_motion = now_motion;
                from_time = time_step;
                now_motion = 0;
                during_time = 40;
                time_step = 0;
                is_lerp = 1;
            }
        }
        
        // 触发各种按键，更新当前记录的数据
        if (Input.GetKeyUp(KeyCode.S))//静止
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;//如果是从下腰转过来的，这部分时间需要长一点
            time_step = 0;
            now_motion = 0;
            is_lerp = 1;
        }
        //前三个动作模式转换中滑步效果明显x
        if (Input.GetKeyUp("1"))//向前走半圈
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 1;
            is_lerp = 1;
        }
        if (Input.GetKeyUp("2"))//后退半圈
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 2;
            is_lerp = 1;
        }
        if (Input.GetKeyUp("3"))//小跑一圈
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 3;
            is_lerp = 1;
        }
        if (Input.GetKeyUp("4"))// 下腰
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 4;
            is_lerp = 1;
        }
        if (Input.GetKeyUp("5"))// 跳绳
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 5;
            is_lerp = 1;
        }
        if (Input.GetKeyUp(KeyCode.O))//向前跑动越过障碍物
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 6;
            is_lerp = 1;
        }
        if (Input.GetKeyUp("7"))
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 7;
            is_lerp = 1;
        }
        //前后左右跳跃
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 8;
            is_lerp = 1;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 9;
            is_lerp = 1;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 10;
            is_lerp = 1;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            org_pos = new Vector3(gameObjects[0].transform.position.x,0.0f,gameObjects[0].transform.position.z);
            if(now_motion<8)org_rot = Quaternion.Euler(new Vector3(0.0f,gameObjects[0].transform.rotation.eulerAngles.y,0.0f));
            last_rot = gameObjects[0].transform.rotation;
            from_motion = now_motion;
            from_time = time_step;
            during_time = 15;
            if(now_motion==4)during_time = 100;
            time_step = 0;
            now_motion = 11;
            is_lerp = 1;
        }
    }
}
