import pandas as pd
import matplotlib.pyplot as plt


def loadData():
    df = pd.read_csv("../../data/timeToFindGoal.csv")

    df.columns = df.columns.str.strip()

    return df


def main():
    df = loadData()

    agg = (
        df.groupby(["MaxHumans", "MaxChairs"])["TimeToFindGoal"]
        .agg(["mean", "std"])
        .reset_index()
    )
    agg["index"] = agg["MaxHumans"].astype(str) + ", " + agg["MaxChairs"].astype(str)

    agg.set_index("index", inplace=True)

    agg.plot(kind="bar", y="mean", yerr="std", capsize=5)
    plt.title("Average time to find goal of pathfinding game")
    plt.xlabel("Number of Humans and Chairs")
    plt.ylabel("Average time to find goal")
    plt.xticks(rotation=45)
    plt.tight_layout()

    plt.savefig("../../data/time.png")

    pass


if __name__ == "__main__":
    main()
