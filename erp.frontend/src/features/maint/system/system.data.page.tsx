import { Table, Typography, Tag } from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../../stores/ui-command.store";
import { getSystemModules, type SystemModuleDto } from "../../../api/system.api";
import UpdateSystemModal from "./update.system.modal";

const { Title } = Typography;

const SystemDataPage = () => {
  const command = useUiCommandStore((s) => s.command);
  const clear = useUiCommandStore((s) => s.clear);

  const [data, setData] = useState<SystemModuleDto[]>([]);
  const [loading, setLoading] = useState(false);

  const [selected, setSelected] = useState<SystemModuleDto | null>(null);
  const [editing, setEditing] = useState<SystemModuleDto | null>(null);

  // QUERY
  useEffect(() => {
    if (command === "QUERY") {
      setLoading(true);
      getSystemModules()
        .then(setData)
        .finally(() => {
          setLoading(false);
          clear();
        });
    }

    // MODIFY = mở modal với dòng đang chọn
    if (command === "MODIFY") {
      if (!selected) return;
      setEditing(selected);
      clear();
    }
  }, [command, clear, selected]);

  // 🔥 UPDATE LOCAL ROW – KHÔNG RELOAD API
  const onUpdated = (updatedItem: SystemModuleDto) => {
    setData((prev) =>
      prev.map((x) => (x.code === updatedItem.code ? updatedItem : x))
    );
  };

  return (
    <>
      <Title level={4}>Dữ liệu hệ thống</Title>

      <Table
        rowKey="code"
        loading={loading}
        dataSource={data}
        size="small"
        rowSelection={{
          type: "radio",
          selectedRowKeys: selected ? [selected.code] : [],
          onChange: (_, rows) => setSelected(rows[0]),
        }}
        columns={[
          {
            title: "Mã hệ thống",
            dataIndex: "code",
            width: 120,
          },
          {
            title: "Tên hệ thống",
            dataIndex: "name",
          },
          {
            title: "Trạng thái",
            dataIndex: "isActive",
            width: 120,
            render: (v) =>
              v ? <Tag color="green">Active</Tag> : <Tag color="red">Inactive</Tag>,
          },
          {
            title: "Cập nhật",
            dataIndex: "updatedAt",
            width: 160,
          },
        ]}
      />

      <UpdateSystemModal
        open={!!editing}
        data={editing}
        onClose={() => setEditing(null)}
        onSuccess={onUpdated}
      />
    </>
  );
};

export default SystemDataPage;
