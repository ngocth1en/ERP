import {
  Table,
  Typography,
  Row,
  Col,
  Input,
  Select,
  Modal,
  Form,
  message,
} from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../stores/ui-command.store";
import { useAuthStore } from "../../auth/auth.store";

import { getBigTypes } from "../../api/asset-big-type.api";
import { getMiddleTypes } from "../../api/asset-middle-type.api";
import { getSmallTypes } from "../../api/asset-small-type.api";
import {
  createAssetModel,
  searchAssetModels,
  updateAssetModel,
} from "../../api/asset-model.api";

const { Title } = Typography;
const PAGE_SIZE = 10;

const AssetModelPage = () => {
  const username = useAuthStore((s) => s.username)!;

  /* ===== TOOLBAR ===== */
  const command = useUiCommandStore((s) => s.command);
  const clearCommand = useUiCommandStore((s) => s.clear);

  /* ===== LOOKUP ===== */
  const [bigTypes, setBigTypes] = useState<any[]>([]);
  const [middleTypes, setMiddleTypes] = useState<any[]>([]);
  const [smallTypes, setSmallTypes] = useState<any[]>([]);

  /* ===== STATE ===== */
  const [filters, setFilters] = useState<any>({
    bigTypeId: "ALL",
    middleTypeId: "ALL",
    smallTypeId: "ALL",
    modelName: "",
  });

  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);

  const [selectedRow, setSelectedRow] = useState<any>();
  const [modalOpen, setModalOpen] = useState(false);
  const [mode, setMode] = useState<"CREATE" | "EDIT">("CREATE");

  const [form] = Form.useForm();

  /* ===== INIT ===== */
  useEffect(() => {
    getBigTypes().then((res) => {
      setBigTypes(res.data);
    });
  }, []);

  /* ===== TOOLBAR COMMAND ===== */
  useEffect(() => {
    if (!command) return;

    switch (command) {
      case "QUERY":
        handleQuery(1);
        break;

      case "INSERT":
        setMode("CREATE");
        form.resetFields();
        setMiddleTypes([]);
        setSmallTypes([]);
        setModalOpen(true);
        break;

      case "MODIFY":
        if (!selectedRow) {
          message.warning("Chọn model");
          break;
        }

        setMode("EDIT");

        getMiddleTypes(selectedRow.bigTypeId).then((r) =>
          setMiddleTypes(r.data)
        );
        getSmallTypes(selectedRow.middleTypeId).then((r) =>
          setSmallTypes(r.data)
        );

        form.setFieldsValue({
          bigTypeId: selectedRow.bigTypeId,
          middleTypeId: selectedRow.middleTypeId,
          smallTypeId: selectedRow.smallTypeId,
          modelName: selectedRow.modelName,
        });

        setModalOpen(true);
        break;
    }

    clearCommand();
  }, [command]);

  /* ===== HANDLER ===== */
  const handleQuery = async (p = page) => {
    setLoading(true);
    try {
      const res = await searchAssetModels({
        bigTypeId:
          filters.bigTypeId === "ALL" ? undefined : filters.bigTypeId,
        middleTypeId:
          filters.middleTypeId === "ALL"
            ? undefined
            : filters.middleTypeId,
        smallTypeId:
          filters.smallTypeId === "ALL"
            ? undefined
            : filters.smallTypeId,
        modelName: filters.modelName || undefined,
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

  const onSave = async () => {
    const values = await form.validateFields();

    if (mode === "CREATE") {
      await createAssetModel(values, username);
      message.success("Thêm model thành công");
      handleQuery(1);
    } else {
      await updateAssetModel(selectedRow.modelId, values, username);
      message.success("Cập nhật model thành công");
      handleQuery(page);
    }

    setModalOpen(false);
  };

  /* ===== RENDER ===== */
  return (
    <>
      <Title level={4}>Quản lý Asset Model</Title>

      {/* ===== FILTER ===== */}
      <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
        {/* BIG */}
        <Col span={6}>
          <Select
            style={{ width: "100%" }}
            dropdownMatchSelectWidth={false}
            value={filters.bigTypeId}
            options={[
              { value: "ALL", label: "ALL" },
              ...bigTypes.map((x) => ({
                value: x.id,
                label: x.name,
              })),
            ]}
            onChange={(v) => {
              if (v === "ALL") {
                setFilters({
                  bigTypeId: "ALL",
                  middleTypeId: "ALL",
                  smallTypeId: "ALL",
                  modelName: filters.modelName,
                });
                setMiddleTypes([]);
                setSmallTypes([]);
              } else {
                setFilters({
                  bigTypeId: v,
                  middleTypeId: "ALL",
                  smallTypeId: "ALL",
                  modelName: filters.modelName,
                });
                getMiddleTypes(v).then((r) => setMiddleTypes(r.data));
              }
            }}
          />
        </Col>

        {/* MIDDLE */}
        <Col span={6}>
          <Select
            style={{ width: "100%" }}
            dropdownMatchSelectWidth={false}
            disabled={filters.bigTypeId === "ALL"}
            value={filters.middleTypeId}
            options={[
              { value: "ALL", label: "ALL" },
              ...middleTypes.map((x) => ({
                value: x.id,
                label: x.name,
              })),
            ]}
            onChange={(v) => {
              if (v === "ALL") {
                setFilters({
                  ...filters,
                  middleTypeId: "ALL",
                  smallTypeId: "ALL",
                });
                setSmallTypes([]);
              } else {
                setFilters({
                  ...filters,
                  middleTypeId: v,
                  smallTypeId: "ALL",
                });
                getSmallTypes(v).then((r) => setSmallTypes(r.data));
              }
            }}
          />
        </Col>

        {/* SMALL */}
        <Col span={6}>
          <Select
            style={{ width: "100%" }}
            dropdownMatchSelectWidth={false}
            disabled={
              filters.bigTypeId === "ALL" ||
              filters.middleTypeId === "ALL"
            }
            value={filters.smallTypeId}
            options={[
              { value: "ALL", label: "ALL" },
              ...smallTypes.map((x) => ({
                value: x.id,
                label: x.name,
              })),
            ]}
            onChange={(v) =>
              setFilters({ ...filters, smallTypeId: v })
            }
          />
        </Col>

        <Col span={6}>
          <Input
            placeholder="Model name"
            allowClear
            value={filters.modelName}
            onChange={(e) =>
              setFilters({ ...filters, modelName: e.target.value })
            }
          />
        </Col>
      </Row>

      {/* ===== TABLE ===== */}
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
          onClick: () => setSelectedRow(r),
        })}
        columns={[
          { title: "Model", dataIndex: "modelName" },
          { title: "Asset", dataIndex: "assetFullName" },
          { title: "Created At", dataIndex: "createdAt" },
        ]}
      />

      {/* ===== MODAL ===== */}
      <Modal
        open={modalOpen}
        title={mode === "CREATE" ? "Thêm Model" : "Sửa Model"}
        onOk={onSave}
        onCancel={() => setModalOpen(false)}
        destroyOnClose
      >
        <Form form={form} layout="vertical">
          <Form.Item
            label="Big Type"
            name="bigTypeId"
            rules={[{ required: true }]}
          >
            <Select
              dropdownMatchSelectWidth={false}
              options={bigTypes.map((x) => ({
                value: x.id,
                label: x.name,
              }))}
              onChange={(v) => {
                form.setFieldValue("middleTypeId", undefined);
                form.setFieldValue("smallTypeId", undefined);
                getMiddleTypes(v).then((r) =>
                  setMiddleTypes(r.data)
                );
              }}
            />
          </Form.Item>

          <Form.Item
            label="Middle Type"
            name="middleTypeId"
            rules={[{ required: true }]}
          >
            <Select
              dropdownMatchSelectWidth={false}
              options={middleTypes.map((x) => ({
                value: x.id,
                label: x.name,
              }))}
              onChange={(v) =>
                getSmallTypes(v).then((r) =>
                  setSmallTypes(r.data)
                )
              }
            />
          </Form.Item>

          <Form.Item
            label="Small Type"
            name="smallTypeId"
            rules={[{ required: true }]}
          >
            <Select
              dropdownMatchSelectWidth={false}
              options={smallTypes.map((x) => ({
                value: x.id,
                label: x.name,
              }))}
            />
          </Form.Item>

          <Form.Item
            label="Model Name"
            name="modelName"
            rules={[{ required: true }]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default AssetModelPage;
