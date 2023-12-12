import matplotlib.pyplot as plt
import pandas as pd


def loadData():
    df = pd.read_csv("../../data/fps.csv")

    df.columns = df.columns.str.strip()

    return df


def main():
    df = loadData()

    mean_fps = (
        df.groupby(["MaxHumans", "MaxChairs"])["Average_FPS"]
        .agg(["mean", "std"])
        .reset_index()
    )
    mean_fps["index"] = (
        mean_fps["MaxHumans"].astype(str) + ", " + mean_fps["MaxChairs"].astype(str)
    )
    mean_fps.set_index("index", inplace=True)

    mean_fps.plot(kind="bar", y="mean", yerr="std", capsize=5)
    plt.title("Average FPS of pathfinding game")
    plt.xlabel("Number of Humans and Chairs")
    plt.ylabel("Average frames per second")
    # rotate x-axis labels
    plt.xticks(rotation=45)
    plt.tight_layout()

    plt.savefig("../../data/fps.png")

    pass


if __name__ == "__main__":
    main()
