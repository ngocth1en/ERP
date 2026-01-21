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
  getAssetBigTypes,
  createAssetBigType,
  updateAssetBigType,
  deleteAssetBigType,
} from "../../api/asset-big-type.api";
import { useAuthStore } from "../../auth/auth.store";

const { Title } = Typography;
const PAGE_SIZE = 10;

interface Props {
  onSelectBigType: (row: any) => void;
  activeTab: "BIG" | "MIDDLE" | "SMALL";
  setActiveTab: (tab: "BIG" | "MIDDLE" | "SMALL") => void;
}



const AssetBigTypeTab = ({ onSelectBigType, activeTab,setActiveTab }: Props) => {
  const command = useUiCommandStore((s) => s.command);
  //const target = useUiCommandStore((s) => s.target);
  const clearCommand = useUiCommandStore((s) => s.clear);

  const username = useAuthStore((s) => s.username);

  const [filters, setFilters] = useState<{ nameEN?: string; nameVN?: string }>({});
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);

  const [selectedRow, setSelectedRow] = useState<any>();
  const [modalOpen, setModalOpen] = useState(false);
  const [mode, setMode] = useState<"CREATE" | "EDIT">("CREATE");

  const [form] = Form.useForm();

  useEffect(() => {
    if (!command) return;
    if (activeTab !== "BIG") return;
    //if (target && target !== "BIG") return;
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
            await deleteAssetBigType(selectedRow.bigTypeId, username!);
            message.success("Đã xóa");
            handleQuery(page);
          },
        });
        break;
    }

    clearCommand();
  }, [command]);

  const handleQuery = async (p = page) => {
    setLoading(true);
    try {
      const res = await getAssetBigTypes({
        nameEN: filters.nameEN,
        nameVN: filters.nameVN,
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
      await createAssetBigType(values, username!);
      message.success("Thêm loại lớn thành công");
      handleQuery(1);
    } else {
      await updateAssetBigType(selectedRow.bigTypeId, values, username!);
      message.success("Cập nhật thành công");
      handleQuery(page);
    }

    setModalOpen(false);
  };

  return (
    <>
      <Title level={4}>Phân loại tài sản – Loại lớn</Title>

      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={6}>
          <Input
            placeholder="Tên EN"
            allowClear
            onChange={(e) =>
              setFilters((p) => ({ ...p, nameEN: e.target.value }))
            }
          />
        </Col>
        <Col span={6}>
          <Input
            placeholder="Tên VN"
            allowClear
            onChange={(e) =>
              setFilters((p) => ({ ...p, nameVN: e.target.value }))
            }
          />
        </Col>
      </Row>

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
        onRow={(record) => ({
          onClick: () => {
            setSelectedRow(record);
            onSelectBigType(record);
            setActiveTab?.("BIG");
          },
          //oubleClick: () => onSelectBigType(record),
        })}
        rowClassName={(record) =>
          record.bigTypeId === selectedRow?.bigTypeId
            ? "ant-table-row-selected"
            : ""
        }
        columns={[
          { title: "STT", dataIndex: "stt", width: 60 },
          { title: "Loại lớn (EN)", dataIndex: "bigTypeNameEN" },
          { title: "Loại lớn (VN)", dataIndex: "bigTypeNameVN" },
          { title: "Người cập nhật", dataIndex: "updatedByName" },
          { title: "Ngày cập nhật", dataIndex: "updatedAt" },
        ]}
      />

      <Modal
        open={modalOpen}
        title={mode === "CREATE" ? "Thêm loại lớn" : "Sửa loại lớn"}
        onOk={onSave}
        onCancel={() => setModalOpen(false)}
      >
        <Form form={form} layout="vertical">
          <Form.Item name="nameEN" label="Tên EN" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="nameVN" label="Tên VN" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default AssetBigTypeTab;
