# 使用手册

谢悦 1900013055



解压该包，使用Unity $2020.3.30f1c1$以上版本打开



## 控制人物方法

开始界面如下，

<img src="Assets\image\65d9536bbe91db11914ac6ea9bdc339.png" alt="65d9536bbe91db11914ac6ea9bdc339" style="zoom: 25%;" />

开始状态为静止。

一开始，首先点击O键，进入跨越障碍物模式。

人物向前走动，跳跃过障碍物。

<img src="Assets\image\1650783655(1).png" alt="1650783655(1)" style="zoom: 33%;" />

之后可以按键使人物自行移动，

人物有五种运动模式：

按1键，人物停下当前动作，向前走半圈，

按2键，人物停下当前动作，倒退走半圈，

按3键，人物停下当前动作，小跑一圈，

按4键，人物停下当前动作，下腰，

按5键，人物停下当前动作，跳绳，

按S键，人物恢复静止状态。

各动作从按键开始反复循环，动作之间随意切换。



此外，为了保证人物前后左右移动，可以，

按Up键，人物停下当前动作，向前小跳，

按Down键，人物停下当前动作，向后小跳，

按Left键，人物停下当前动作，向左小跳，

按Right键，人物停下当前动作，向右小跳，



## 使用数据集清单

采用SFU数据集中Subject0005，0015和0018。

共有：

[0015_HopOverObstacle001.bvh](https://mocap.cs.sfu.ca/nusmocap/0015_HopOverObstacle001.bvh)

[0018_Bridge001.bvh](https://mocap.cs.sfu.ca/nusmocap/0018_Bridge001.bvh)

[0005_2FeetJump001.bvh](https://mocap.cs.sfu.ca/nusmocap/0005_2FeetJump001.bvh)

[0005_BackwardsWalk001.bvh](https://mocap.cs.sfu.ca/nusmocap/0005_BackwardsWalk001.bvh)

[0005_Jogging001.bvh](https://mocap.cs.sfu.ca/nusmocap/0005_Jogging001.bvh)

[0005_JumpRope001.bvh](https://mocap.cs.sfu.ca/nusmocap/0005_JumpRope001.bvh)

[0005_SlowTrot001.bvh](https://mocap.cs.sfu.ca/nusmocap/0005_SlowTrot001.bvh)

[0005_Stomping001.bvh](https://mocap.cs.sfu.ca/nusmocap/0005_Stomping001.bvh)

[0005_Walking001.bvh](https://mocap.cs.sfu.ca/nusmocap/0005_Walking001.bvh)



## 代码 

BVHLoader.cs 是操控人物运动的代码

BVH文件夹下，全是使用到的动作数据

主要有

static.bvh

Walking.bvh

BackwardsWalk.bvh

SlowTrot.bvh

Body.bvh

JumpRope.bvh

Obstacle.bvh

static8.bvh

up.bvh

down.bvh

left.bvh

right.bvh

等bvh文件