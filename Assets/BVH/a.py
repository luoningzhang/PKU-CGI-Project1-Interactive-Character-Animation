# 实现PCA分析和法向量计算，并加载数据集中的文件进行验证

import open3d as o3d 
# import os
import numpy as np
from scipy.spatial import KDTree

# from pyntcloud import PyntCloud

# 功能：计算PCA的函数
# 输入：
#     data：点云，NX3的矩阵
#     correlation：区分np的cov和corrcoef，不输入时默认为False
#     sort: 特征值排序，排序是为了其他功能方便使用，不输入时默认为True
# 输出：
#     eigenvalues：特征值
#     eigenvectors：特征向量
def PCA(data, correlation=False, sort=True):
    # normalize 归一化
    mean_data = np.mean(data, axis=0)
    normal_data = data - mean_data
    # 计算对称的协方差矩阵
    H = np.dot(normal_data.T, normal_data)
    # SVD奇异值分解，得到H矩阵的特征值和特征向量
    eigenvectors, eigenvalues, _ = np.linalg.svd(H)

    if sort:
        sort = eigenvalues.argsort()[::-1]
        eigenvalues = eigenvalues[sort]
        eigenvectors = eigenvectors[:, sort]

    return eigenvalues, eigenvectors

def main():

    # 从txt文件获取点，只对点进行处理
    filename = "data.txt"
    points = np.loadtxt(filename)[:, 0:3] # 导入txt数据到np.array，这里只需导入前3列
    print('total points number is:', points.shape[0])
    print(points)

    # 用PCA分析点云主方向
    w, v = PCA(points) # PCA方法得到对应的特征值和特征向量
    point_cloud_vector = v[:, 0] #点云主方向对应的向量为最大特征值对应的特征向量
    print('the main orientation of this pointcloud is: ', point_cloud_vector)
    # 三个特征向量组成了三个坐标轴
    axis = o3d.geometry.TriangleMesh.create_coordinate_frame().rotate(v, center=(0, 0, 0))

    # 循环计算每个点的法向量
    leafsize = 32   # 切换为暴力搜索的最小数量
    KDTree_radius = 0.1 # 设置邻域半径
    tree = KDTree(points, leafsize=leafsize) # 构建KDTree
    radius_neighbor_idx = tree.query_ball_point(points, KDTree_radius) # 得到每个点的邻近索引
    normals = [] # 定义一个空list

    # -------------寻找法线---------------
    # 首先寻找邻域内的点
    for i in range(len(radius_neighbor_idx)):
        neighbor_idx = radius_neighbor_idx[i] # 得到第i个点的邻近点索引，邻近点包括自己
        neighbor_data = points[neighbor_idx] # 得到邻近点，在求邻近法线时没必要归一化，在PCA函数中归一化就行了
        eigenvalues, eigenvectors = PCA(neighbor_data) # 对邻近点做PCA，得到特征值和特征向量
        normals.append(eigenvectors[:, 2]) # 最小特征值对应的方向就是法线方向
    # ------------法线查找结束---------------
    normals = np.array(normals, dtype=np.float64) # 把法线放在了normals中
    # o3d.geometry.PointCloud，返回了PointCloud类型
    pc_view = o3d.geometry.PointCloud(points=o3d.utility.Vector3dVector(points))
    # 向PointCloud对象中添加法线
    pc_view.normals = o3d.utility.Vector3dVector(normals)
    # 可视化
    o3d.visualization.draw_geometries([pc_view, axis], point_show_normal=True)

if __name__ == '__main__':
    main()

