import {
  Table,
  Typography,
  Row,
  Col,
  Input,
  Select,
  Modal,
  Form,
  Switch,
  message,
} from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../stores/ui-command.store";
import {
  createStorageLocation,
  searchStorageLocations,
  updateStorageLocation,
} from "../../api/location.api";
import { getDepartmentOptions } from "../../api/department.api";

const { Title } = Typography;
const PAGE_SIZE = 10;

/* ======================
   CONST
====================== */
const FLOOR_OPTIONS = [
  { value: 1, label: "Tầng 1" },
  { value: 2, label: "Tầng 2" },
  { value: 3, label: "Tầng 3" },
  { value: 4, label: "Tầng 4" },
  { value: 5, label: "Tầng 5" },
  { value: 6, label: "Tầng 6" },
];

/* ======================
   COMPONENT
====================== */
const LocationPage = () => {
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

  const [departments, setDepartments] = useState<any[]>([]);

  const [form] = Form.useForm();

  /* ===== LOAD MASTER ===== */
  useEffect(() => {
    getDepartmentOptions().then((res) => setDepartments(res.data));
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
        setModalOpen(true);
        break;

      case "MODIFY":
        if (!selectedRow) {
          message.warning("Chọn vị trí");
          break;
        }
        setMode("EDIT");
        form.setFieldsValue({
          name: selectedRow.nameVN,
          status: selectedRow.status,
          inUse: selectedRow.inUse === 1,
          departmentId: selectedRow.departmentId,
          floor: selectedRow.floor,
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
      const res = await searchStorageLocations({
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

  const onSave = async () => {
    const values = await form.validateFields();

    if (mode === "CREATE") {
      await createStorageLocation({
        ...values,
        inUse: values.inUse ? 1 : 0,
      });
      message.success("Thêm vị trí thành công");
      handleQuery(1);
    } else {
      await updateStorageLocation(selectedRow.storageLocationNo, {
        name: values.name,
        status: values.status,
        inUse: values.inUse ? 1 : 0,
      });
      message.success("Cập nhật thành công");
      handleQuery(page);
    }

    setModalOpen(false);
  };

  /* ===== RENDER ===== */
  return (
    <>
      <Title level={4}>Quản lý vị trí</Title>

      {/* ===== FILTER ===== */}
      <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
        <Col xs={24} sm={12} md={6}>
          <Input
            placeholder="Tên vị trí"
            allowClear
            onChange={(e) =>
              setFilters({ ...filters, name: e.target.value })
            }
          />
        </Col>

        <Col xs={24} sm={12} md={6}>
          <Select
            placeholder="Bộ phận"
            allowClear
            style={{ width: "100%" }}
            onChange={(v) =>
              setFilters({ ...filters, departmentId: v })
            }
            options={departments.map((d) => ({
              value: d.id,
              label: d.name,
            }))}
          />
        </Col>

        <Col xs={24} sm={12} md={6}>
          <Select
            placeholder="Trạng thái"
            allowClear
            style={{ width: "100%" }}
            onChange={(v) =>
              setFilters({ ...filters, status: v })
            }
            options={[
              { value: 1, label: "U - Đang dùng" },
              { value: 2, label: "N - Trống" },
              { value: 3, label: "S - Báo phế" },
            ]}
          />
        </Col>

        <Col xs={24} sm={12} md={6}>
          <Select
            placeholder="InUse"
            allowClear
            style={{ width: "100%" }}
            onChange={(v) =>
              setFilters({ ...filters, inUse: v })
            }
            options={[
              { value: 1, label: "Đang dùng" },
              { value: 0, label: "Không dùng" },
            ]}
          />
        </Col>
      </Row>

      {/* ===== TABLE ===== */}
      <Table
        rowKey="storageLocationNo"
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
          { title: "Mã vị trí", dataIndex: "storageLocationNo" },
          { title: "Tên vị trí", dataIndex: "nameVN" },
          { title: "Bộ phận", dataIndex: "departmentName" },
          {
            title: "Trạng thái",
            render: (_, r) =>
              r.status === 1 ? "U" : r.status === 2 ? "N" : "S",
          },
          {
            title: "InUse",
            render: (_, r) => (r.inUse === 1 ? "✔" : ""),
          },
        ]}
      />

      {/* ===== MODAL ===== */}
      <Modal
        open={modalOpen}
        title={mode === "CREATE" ? "Thêm vị trí" : "Sửa vị trí"}
        onOk={onSave}
        onCancel={() => setModalOpen(false)}
        destroyOnClose
      >
        <Form form={form} layout="vertical">
          <Form.Item
            label="Bộ phận"
            name="departmentId"
            rules={[{ required: true, message: "Chọn bộ phận" }]}
          >
            <Select
              placeholder="Chọn bộ phận"
              options={departments.map((d) => ({
                value: d.id,
                label: d.name,
              }))}
            />
          </Form.Item>

          <Form.Item
            label="Tầng"
            name="floor"
            rules={[{ required: true, message: "Chọn tầng" }]}
          >
            <Select
              placeholder="Chọn tầng"
              options={FLOOR_OPTIONS}
            />
          </Form.Item>

          <Form.Item
            label="Tên vị trí"
            name="name"
            rules={[{ required: true, message: "Nhập tên vị trí" }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            label="Trạng thái"
            name="status"
            rules={[{ required: true }]}
          >
            <Select
              options={[
                { value: 1, label: "U - Đang dùng" },
                { value: 2, label: "N - Trống" },
                { value: 3, label: "S - Báo phế" },
              ]}
            />
          </Form.Item>

          <Form.Item
            label="InUse"
            name="inUse"
            valuePropName="checked"
          >
            <Switch />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default LocationPage;
