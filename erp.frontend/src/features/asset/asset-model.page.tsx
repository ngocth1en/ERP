import {
  Button,
  Col,
  Form,
  Input,
  Modal,
  Row,
  Select,
  Table,
  Checkbox,
  message,
} from "antd";
import { useEffect, useState } from "react";
import { useAuthStore } from "../../auth/auth.store";
import { getBigTypes } from "../../api/asset-big-type.api";
import { getMiddleTypes } from "../../api/asset-middle-type.api";
import { getSmallTypes } from "../../api/asset-small-type.api";
import { createAssetModel, searchAssetModels, updateAssetModel } from "../../api/asset-model.api";

const PAGE_SIZE = 10;

export default function AssetModelPage() {
  const username = useAuthStore((s) => s.username)!;

  /* ================= LOOKUP ================= */
  const [bigTypes, setBigTypes] = useState<any[]>([]);
  const [middleTypes, setMiddleTypes] = useState<any[]>([]);
  const [smallTypes, setSmallTypes] = useState<any[]>([]);

  /* ================= SEARCH ================= */
  const [bigTypeId, setBigTypeId] = useState<number | "ALL">("ALL");
  const [middleTypeId, setMiddleTypeId] = useState<number>();
  const [smallTypeId, setSmallTypeId] = useState<number>();
  const [modelName, setModelName] = useState("");

  const [data, setData] = useState<any[]>([]);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);

  /* ================= MODAL ================= */
  const [openCreate, setOpenCreate] = useState(false);
  const [openUpdate, setOpenUpdate] = useState(false);
  const [current, setCurrent] = useState<any>();

  const [formCreate] = Form.useForm();
  const [formUpdate] = Form.useForm();

  /* ================= INIT ================= */
  useEffect(() => {
    getBigTypes().then((res) => setBigTypes(res.data));
  }, []);

  /* ================= CASCADE ================= */

  const onBigChange = (value: number | "ALL") => {
    setBigTypeId(value);
    setMiddleTypeId(undefined);
    setSmallTypeId(undefined);
    setMiddleTypes([]);
    setSmallTypes([]);

    if (value !== "ALL") {
      getMiddleTypes(value).then((res) => setMiddleTypes(res.data));
    }
  };

  const onMiddleChange = (value: number) => {
    setMiddleTypeId(value);
    setSmallTypeId(undefined);
    setSmallTypes([]);

    getSmallTypes(value).then((res) => setSmallTypes(res.data));
  };

  /* ================= SEARCH ================= */

  const onSearch = async (p = page) => {
    setLoading(true);

    const res = await searchAssetModels({
      bigTypeId: bigTypeId === "ALL" ? undefined : bigTypeId,
      middleTypeId: bigTypeId === "ALL" ? undefined : middleTypeId,
      smallTypeId: bigTypeId === "ALL" ? undefined : smallTypeId,
      modelName: modelName || undefined,
      page: p,
      pageSize: PAGE_SIZE,
    });

    setData(res.items);
    setTotal(res.total);
    setPage(p);
    setLoading(false);
  };

  /* ================= CREATE ================= */

  const onCreate = async () => {
    const v = await formCreate.validateFields();

    await createAssetModel(
      {
        bigTypeId: v.bigTypeId,
        middleTypeId: v.middleTypeId,
        smallTypeId: v.smallTypeId,
        modelName: v.modelName,
      },
      username
    );

    message.success("Tạo model thành công");
    setOpenCreate(false);
    onSearch(1);
  };

  /* ================= UPDATE ================= */

  const openUpdateModal = (row: any) => {
    setCurrent(row);
    formUpdate.setFieldsValue({
      ...row,
      updateImage: false,
    });
    setOpenUpdate(true);
  };

  const onUpdate = async () => {
    const v = await formUpdate.validateFields();

    await updateAssetModel(
      current.modelId,
      {
        bigTypeId: v.bigTypeId,
        middleTypeId: v.middleTypeId,
        smallTypeId: v.smallTypeId,
        modelName: v.modelName,
        updateImage: v.updateImage,
      },
      username
    );

    message.success("Cập nhật thành công");
    setOpenUpdate(false);
    onSearch();
  };

  /* ================= UI ================= */

  return (
    <>
      {/* SEARCH */}
      <Row gutter={12} style={{ marginBottom: 12 }}>
        <Col span={5}>
          <Select
            value={bigTypeId}
            onChange={onBigChange}
            style={{ width: "100%" }}
            options={[
              { value: "ALL", label: "ALL" },
              ...bigTypes.map((x) => ({
                value: x.id,
                label: x.nameVN,
              })),
            ]}
          />
        </Col>

        <Col span={5}>
          <Select
            value={middleTypeId}
            disabled={bigTypeId === "ALL"}
            onChange={onMiddleChange}
            options={middleTypes.map((x) => ({
              value: x.id,
              label: x.nameVN,
            }))}
          />
        </Col>

        <Col span={5}>
          <Select
            value={smallTypeId}
            disabled={bigTypeId === "ALL"}
            onChange={setSmallTypeId}
            options={smallTypes.map((x) => ({
              value: x.id,
              label: x.nameVN,
            }))}
          />
        </Col>

        <Col span={5}>
          <Input
            placeholder="Model name"
            value={modelName}
            onChange={(e) => setModelName(e.target.value)}
          />
        </Col>

        <Col span={4}>
          <Button type="primary" onClick={() => onSearch(1)}>
            Search
          </Button>
          <Button style={{ marginLeft: 8 }} onClick={() => setOpenCreate(true)}>
            Add
          </Button>
        </Col>
      </Row>

      {/* TABLE */}
      <Table
        rowKey="modelId"
        loading={loading}
        dataSource={data}
        pagination={{
          current: page,
          pageSize: PAGE_SIZE,
          total,
          onChange: onSearch,
        }}
        columns={[
          { title: "Model", dataIndex: "modelName" },
          { title: "Asset", dataIndex: "assetFullName" },
          { title: "Created At", dataIndex: "createdAt" },
          {
            title: "Action",
            render: (_, r) => (
              <Button onClick={() => openUpdateModal(r)}>Edit</Button>
            ),
          },
        ]}
      />

      {/* CREATE MODAL */}
      <Modal
        open={openCreate}
        onCancel={() => setOpenCreate(false)}
        onOk={onCreate}
        title="Create Asset Model"
      >
        <Form form={formCreate} layout="vertical">
          <Form.Item name="bigTypeId" label="Big Type" rules={[{ required: true }]}>
            <Select
              onChange={(v) => {
                formCreate.setFieldValue("middleTypeId", undefined);
                formCreate.setFieldValue("smallTypeId", undefined);
                getMiddleTypes(v).then((r) => setMiddleTypes(r.data));
              }}
              options={bigTypes.map((x) => ({
                value: x.id,
                label: x.nameVN,
              }))}
            />
          </Form.Item>

          <Form.Item
            name="middleTypeId"
            label="Middle Type"
            rules={[{ required: true }]}
          >
            <Select
              onChange={(v) =>
                getSmallTypes(v).then((r) => setSmallTypes(r.data))
              }
              options={middleTypes.map((x) => ({
                value: x.id,
                label: x.nameVN,
              }))}
            />
          </Form.Item>

          <Form.Item
            name="smallTypeId"
            label="Small Type"
            rules={[{ required: true }]}
          >
            <Select
              options={smallTypes.map((x) => ({
                value: x.id,
                label: x.nameVN,
              }))}
            />
          </Form.Item>

          <Form.Item name="modelName" label="Model Name" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
        </Form>
      </Modal>

      {/* UPDATE MODAL */}
      <Modal
        open={openUpdate}
        onCancel={() => setOpenUpdate(false)}
        onOk={onUpdate}
        title="Update Asset Model"
      >
        <Form form={formUpdate} layout="vertical">
          <Form.Item name="modelName" label="Model Name" rules={[{ required: true }]}>
            <Input />
          </Form.Item>

          <Form.Item name="updateImage" valuePropName="checked">
            <Checkbox>Update Image</Checkbox>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
}
