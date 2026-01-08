import { Table, Typography } from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../../stores/ui-command.store";
import { getSystems } from "../../../api/system.api";

const { Title } = Typography;

export const SystemGuidePage = () => {
  const command = useUiCommandStore((s) => s.command);
  const clear = useUiCommandStore((s) => s.clear);

  const [systems, setSystems] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (command !== "QUERY") return;

    setLoading(true);

    getSystems()
      .then(setSystems)
      .finally(() => {
        setLoading(false);
        clear(); // rất quan trọng
      });
  }, [command]);

  return (
    <>
      <Title level={4}>Hướng dẫn sử dụng hệ thống</Title>

      <Table
        rowKey="code"
        loading={loading}
        dataSource={systems}
        columns={[
          {
            title: "Tên hệ thống",
            dataIndex: "name",
          },
          {
            title: "Mã hệ thống",
            dataIndex: "code",
            hidden: true,
          },
        ]}
      />
    </>
  );
};
