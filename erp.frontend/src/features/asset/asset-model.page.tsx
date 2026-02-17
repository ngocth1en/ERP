import {
  Table,
  Typography,
  Row,
  Col,
  Select,
  Tabs,
} from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../stores/ui-command.store";

import { getBigTypes } from "../../api/asset-big-type.api";
import { getMiddleTypes } from "../../api/asset-middle-type.api";
import {
  getDepartmentOptions,
  type DepartmentOptionDto,
} from "../../api/department.api";

import {
  searchStatistic,
  searchAssetsByModel,
} from "../../api/asset-information.api";

const { Title } = Typography;
const { TabPane } = Tabs;
const PAGE_SIZE = 10;

const AssetInformationPage = () => {
  /* ===== TOOLBAR ===== */
  const command = useUiCommandStore((s) => s.command);
  const clearCommand = useUiCommandStore((s) => s.clear);

  /* ===== LOOKUP ===== */
  const [bigTypes, setBigTypes] = useState<any[]>([]);
  const [middleTypes, setMiddleTypes] = useState<any[]>([]);
  const [departments, setDepartments] = useState<DepartmentOptionDto[]>([]);

  /* ===== FILTER ===== */
  const [filters, setFilters] = useState<{
    bigTypeId: "ALL" | number;
    middleTypeId: "ALL" | number;
    departmentId: "ALL" ;
  }>({
    bigTypeId: "ALL",
    middleTypeId: "ALL",
    departmentId: "ALL",
  });

  /* ===== TAB 1 STATE ===== */
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);

  /* ===== TAB 2 STATE ===== */
  const [selectedModel, setSelectedModel] = useState<any>();
  const [detailData, setDetailData] = useState<any[]>([]);
  const [detailLoading, setDetailLoading] = useState(false);

  /* ===== INIT ===== */
  useEffect(() => {
    getBigTypes().then((r) => setBigTypes(r.data));
    getDepartmentOptions().then((r) =>
      setDepartments(r.data)
    );
  }, []);

  /* ===== TOOLBAR COMMAND ===== */
  useEffect(() => {
    if (!command) return;

    switch (command) {
      case "QUERY":
        handleQuery(1);
        break;
    }

    clearCommand();
  }, [command]);

  /* ===== QUERY TAB 1 ===== */
  const handleQuery = async (p = page) => {
    setLoading(true);
    try {
      const res = await searchStatistic({
        bigTypeId:
          filters.bigTypeId === "ALL"
            ? undefined
            : filters.bigTypeId,
        middleTypeId:
          filters.middleTypeId === "ALL"
            ? undefined
            : filters.middleTypeId,
        departmentId:
          filters.departmentId === "ALL"
            ? undefined
            : filters.departmentId,
        page: p,
        pageSize: PAGE_SIZE,
      });

      setData(res.items);
      setTotal(res.total);
      setPage(p);
    } finally {
      setLoading(false);
    }
  };

  /* ===== LOAD DETAIL (TAB 2) ===== */
  const openDetail = async (row: any) => {
    setSelectedModel(row);
    setDetailLoading(true);

    try {
      const res = await searchAssetsByModel({
        modelId: row.modelId,
        page: 1,
        pageSize: 100,
      });
      setDetailData(res.items);
    } finally {
      setDetailLoading(false);
    }
  };

  /* ===== RENDER ===== */
  return (
    <>
      <Title level={4}>Asset Information</Title>

      <Tabs defaultActiveKey="1">
        {/* ================= TAB 1 ================= */}
        <TabPane tab="Thống kê theo Model" key="1">
          {/* ===== FILTER ===== */}
          <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
            {/* Big Type */}
            <Col span={6}>
              <Select
                style={{ width: "100%" }}
                value={filters.bigTypeId}
                options={[
                  { value: "ALL", label: "ALL" },
                  ...bigTypes.map((x) => ({
                    value: x.id,
                    label: x.name,
                  })),
                ]}
                onChange={(v) => {
                  setFilters({
                    ...filters,
                    bigTypeId: v,
                    middleTypeId: "ALL",
                  });

                  if (v !== "ALL") {
                    getMiddleTypes(v).then((r) =>
                      setMiddleTypes(r.data)
                    );
                  } else {
                    setMiddleTypes([]);
                  }
                }}
              />
            </Col>

            {/* Middle Type */}
            <Col span={6}>
              <Select
                style={{ width: "100%" }}
                disabled={filters.bigTypeId === "ALL"}
                value={filters.middleTypeId}
                options={[
                  { value: "ALL", label: "ALL" },
                  ...middleTypes.map((x) => ({
                    value: x.id,
                    label: x.name,
                  })),
                ]}
                onChange={(v) =>
                  setFilters({
                    ...filters,
                    middleTypeId: v,
                  })
                }
              />
            </Col>

            {/* Department */}
            <Col span={6}>
              <Select
                style={{ width: "100%" }}
                value={filters.departmentId}
                options={[
                  { value: "ALL", label: "ALL" },
                  ...departments.map((x) => ({
                    value: x.id,
                    label: x.name,
                  })),
                ]}
                onChange={(v) =>
                  setFilters({
                    ...filters,
                    departmentId: v,
                  })
                }
              />
            </Col>
          </Row>

          {/* ===== TABLE TAB 1 ===== */}
          <Table
            rowKey="modelId"
            loading={loading}
            dataSource={data}
            pagination={{
              current: page,
              pageSize: PAGE_SIZE,
              total,
              onChange: handleQuery,
            }}
            onRow={(r) => ({
              onClick: () => openDetail(r),
            })}
            columns={[
              {
                title: "Asset",
                dataIndex: "assetName",
                ellipsis: true,
              },
              {
                title: "Model",
                dataIndex: "modelName",
                ellipsis: true,
              },
              {
                title: "Phòng ban",
                dataIndex: "departmentId",
              },
              {
                title: "Tổng",
                dataIndex: "totalAssets",
              },
            ]}
          />
        </TabPane>

        {/* ================= TAB 2 ================= */}
        <TabPane tab="Danh sách Asset trong Model" key="2">
          {!selectedModel ? (
            <div>Vui lòng chọn 1 Model ở Tab 1</div>
          ) : (
            <Table
              rowKey="assetNo"
              loading={detailLoading}
              dataSource={detailData}
              pagination={false}
              columns={[
                { title: "Asset No", dataIndex: "assetNo" },
                {
                  title: "Asset",
                  dataIndex: "assetName",
                  ellipsis: true,
                },
                {
                  title: "Model",
                  dataIndex: "modelName",
                  ellipsis: true,
                },
                {
                  title: "Phòng ban",
                  dataIndex: "departmentId",
                },
                {
                  title: "Trạng thái",
                  dataIndex: "status",
                  render: (v) =>
                    v === 1 ? "Đang dùng" : "Ngưng dùng",
                },
              ]}
            />
          )}
        </TabPane>
      </Tabs>
    </>
  );
};

export default AssetInformationPage;
