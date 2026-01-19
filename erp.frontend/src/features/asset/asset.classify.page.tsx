import {
  Table,
  Typography,
  Row,
  Col,
  Input,
  Modal,
  Form,
  message,
} from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../stores/ui-command.store";
import {
  searchAssetBigTypes,
  createAssetBigType,
  updateAssetBigType,
  deleteAssetBigType,
} from "../../api/asset-big-type.api";

const { Title } = Typography;
const PAGE_SIZE = 10;

const AssetClassifyPage = () => {
  /* ===== TOOLBAR ===== */
  const command = useUiCommandStore((s) => s.command);
  const clearCommand = useUiCommandStore((s) => s.clear);

  /* ===== STATE ===== */
  const [filters, setFilters] = useState<any>({});
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);

  const [selectedRow, setSelectedRow] = useState<any>();
  const [modalOpen, setModalOpen] = useState(false);
  const [mode, setMode] = useState<"CREATE" | "EDIT">("CREATE");

  const [form] = Form.useForm();

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
        setModalOpen(true);
        break;

      case "MODIFY":
        if (!selectedRow) {
          message.warning("Vui lòng chọn dữ liệu");
          break;
        }
        setMode("EDIT");
        form.setFieldsValue({
          nameEN: selectedRow.bigTypeNameEN,
          nameVN: selectedRow.bigTypeNameVN,
        });
        setModalOpen(true);
        break;

      case "DELETE":
        if (!selectedRow) {
          message.warning("Vui lòng chọn dữ liệu");
          break;
        }
        Modal.confirm({
          title: "Xác nhận xóa?",
          onOk: async () => {
            await deleteAssetBigType(selectedRow.bigTypeId);
            message.success("Đã xóa");
            handleQuery(page);
          },
        });
        break;
    }

    clearCommand();
  }, [command]);

  /* ===== QUERY ===== */
  const handleQuery = async (p = page) => {
    setLoading(true);
    try {
      const res = await searchAssetBigTypes({
        ...filters,
        page: p,
        pageSize: PAGE_SIZE,
      });

      setData(res.data.items);
      setTotal(res.data.total);
      setPage(p);
    } finally {
      setLoading(false);
    }
  };

  /* ===== SAVE ===== */
  const onSave = async () => {
    const values = await form.validateFields();

    if (mode === "CREATE") {
      await createAssetBigType(values);
      message.success("Thêm loại lớn thành công");
      handleQuery(1);
    } else {
      await updateAssetBigType(selectedRow.bigTypeId, values);
      message.success("Cập nhật thành công");
      handleQuery(page);
    }

    setModalOpen(false);
  };

  /* ===== RENDER ===== */
  return (
    <>
      <Title level={4}>Loại thiết bị lớn</Title>

      {/* ===== FILTER ===== */}
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={6}>
          <Input
            placeholder="Tên EN"
            allowClear
            onChange={(e) =>
              setFilters({ ...filters, nameEN: e.target.value })
            }
          />
        </Col>

        <Col span={6}>
          <Input
            placeholder="Tên VN"
            allowClear
            onChange={(e) =>
              setFilters({ ...filters, nameVN: e.target.value })
            }
          />
        </Col>
      </Row>

      {/* ===== TABLE ===== */}
      <Table
        rowKey="bigTypeId"
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
          { title: "STT", dataIndex: "stt", width: 60 },
          {
            title: "Loại lớn (Anh)",
            dataIndex: "bigTypeNameEN",
          },
          {
            title: "Loại lớn (Việt)",
            dataIndex: "bigTypeNameVN",
          },
          {
            title: "Người dùng",
            dataIndex: "updatedByName",
          },
          {
            title: "Ngày cập nhật",
            dataIndex: "updatedAt",
          },
        ]}
      />

      {/* ===== MODAL ===== */}
      <Modal
        open={modalOpen}
        title={mode === "CREATE" ? "Thêm loại lớn" : "Sửa loại lớn"}
        onOk={onSave}
        onCancel={() => setModalOpen(false)}
        destroyOnClose
      >
        <Form form={form} layout="vertical">
          <Form.Item
            label="Tên (Anh)"
            name="nameEN"
            rules={[{ required: true, message: "Nhập tên EN" }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            label="Tên (Việt)"
            name="nameVN"
            rules={[{ required: true, message: "Nhập tên VN" }]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default AssetClassifyPage;
