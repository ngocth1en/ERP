import { Tabs } from "antd";
import { useState } from "react";
import AssetBigTypeTab from "./asset.big.type.tab";
import AssetMiddleTypeTab from "./asset.middle.type.tab";

const AssetClassifyPage = () => {
  const [selectedBigType, setSelectedBigType] = useState<any>();
  const [activeTab, setActiveTab] = useState<"BIG" | "MIDDLE" | "SMALL">("BIG");

  return (
    <Tabs activeKey={activeTab === "BIG" ? "big" : activeTab === "MIDDLE" ? "middle" : "small"}
      onChange={(k) =>
        setActiveTab(k === "big" ? "BIG" : k === "middle" ? "MIDDLE" : "SMALL")
      }
      items={[
        {
          key: "big",
          label: "Loại lớn",
          children: (
            <AssetBigTypeTab
              onSelectBigType={(row) => setSelectedBigType(row)}
              activeTab={activeTab}
              setActiveTab={setActiveTab}
            />
          ),
        },
        {
          key: "middle",
          label: "Loại vừa",
          children: (
            <AssetMiddleTypeTab
              bigType={selectedBigType}
              activeTab={activeTab}
              setActiveTab={setActiveTab}
            />
          ),
        },
      ]}
    />
  );
};

export default AssetClassifyPage;
