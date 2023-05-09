import requests
import sys
import plotly.express as px
import pandas as pd

program_args = sys.argv
if len(program_args) < 2:
    print("Please provide a csv file path and k value")
    exit(1)

SEPARATOR = ";"
K = program_args[1]
CSV_FILE_PATH = program_args[2]
DISPLAY_IN_3D = False

print(f"K: {K}")
print(f"CSV_FILE_PATH: {CSV_FILE_PATH}")

API_ADDRESS = f"https://localhost:7097/api/ClusterKMeans/init?k={K}&csvSeparator={SEPARATOR}"

symbols_dict = {
    "Centroid": "diamond"
}

[symbols_dict.update({f"Cluster {i}": "circle"}) for i in range(1, int(K) + 1)]


def visualize_clusters_with_animation(response_json, in_3d=True):
    vectors = []
    current_iteration = 0
    for iteration in response_json["History"]:
        for cluster in iteration["Clusters"]:
            for vector in cluster["Vectors"]:
                vector["ClusterName"] = cluster["ClusterName"]
                vector["Iteration"] = current_iteration
                vectors.append(vector)
        current_iteration += 1

    current_iteration = 0
    for iteration in response_json["History"]:
        for cluster in iteration["Clusters"]:
            cluster["Centroid"]["ClusterName"] = "Centroid"
            cluster["Centroid"]["label"] = cluster["ClusterName"] + " Centroid"
            cluster["Centroid"]["Iteration"] = current_iteration
            vectors.append(cluster["Centroid"])
        current_iteration += 1

    df = pd.DataFrame(vectors)
    df[['sepal_length', 'sepal_width', 'petal_length', 'petal_width']] = pd.DataFrame(df.attributes.tolist())
    df = df.drop(columns=['attributes'])
    if in_3d:
        fig = px.scatter_3d(df, x="sepal_length", y="sepal_width", z='petal_length', color="ClusterName",
                            color_discrete_map={"Centroid": "black"},
                            symbol="ClusterName", symbol_map=symbols_dict,
                            hover_name="label", animation_frame="Iteration", animation_group="label",
                            title="3d Iris Scatter Plot with Animation")
    else:
        fig = px.scatter(df, x="sepal_length", y="petal_length", color="ClusterName",
                         color_discrete_map={"Centroid": "black"},
                         symbol="ClusterName", symbol_map=symbols_dict,
                         hover_name="label", animation_frame="Iteration", animation_group="label",
                         title="2d Iris Scatter Plot with Animation")

    fig.update_traces(marker=dict(size=12,
                                  line=dict(width=2, color='DarkSlateGrey')
                                  ),
                      selector=dict(mode='markers'))
    fig.show()


def visualize_clusters(response_json, in_3d=True):
    vectors = []
    for cluster in response_json["Clusters"]:
        cluster["Centroid"]["ClusterName"] = "Centroid"
        vectors.append(cluster["Centroid"])
        for vector in cluster["Vectors"]:
            vector["ClusterName"] = cluster["ClusterName"]
            cluster["Centroid"]["label"] = cluster["ClusterName"] + " Centroid"
            vectors.append(vector)

    df = pd.DataFrame(vectors)
    df[['sepal_length', 'sepal_width', 'petal_length', 'petal_width']] = pd.DataFrame(df.attributes.tolist())
    df = df.drop(columns=['attributes'])
    if in_3d:
        fig = px.scatter_3d(df, x="sepal_length", y="sepal_width", z='petal_length', color="ClusterName",
                            hover_name="label",
                            title="3d Iris Scatter Plot")
    else:
        fig = px.scatter(df, x="sepal_length", y="petal_length", color="ClusterName",
                         hover_name="label",
                         title="2d Iris Scatter Plot")
    fig.update_traces(marker=dict(size=12, line=dict(width=2, color='DarkSlateGrey')), selector=dict(mode='markers'))
    fig.show()


def main():
    headers = {
        'accept': '*/*',
    }
    params = (
        ('k', K),
        ('csvSeparator', SEPARATOR),
    )

    files = {
        'inputFile': open(CSV_FILE_PATH, 'rb'),
    }
    response = requests.post('https://localhost:7097/api/ClusterKMeans/init', headers=headers, params=params,
                             files=files, verify=False)

    response_json = response.json()

    current_iteration = 0
    for iteration in response_json["History"]:
        print(f"\tIteration: {current_iteration}")
        for cluster in iteration["Clusters"]:
            print(f"{cluster['ClusterName']}, number of vectors: {len(cluster['Vectors'])}, "
                  f"SSE: {'{:.2f}'.format(cluster['SumOfSquaredErrors'])}, centroid: {cluster['Centroid']}")
        current_iteration += 1

    visualize_clusters_with_animation(response_json, in_3d=DISPLAY_IN_3D)


if __name__ == '__main__':
    main()
