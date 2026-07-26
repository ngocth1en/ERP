import { Table, Typography, Row, Col } from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../../stores/ui-command.store";
import { getSystemGuideFiles, getSystems } from "../../../api/system.api";

const { Title } = Typography;

const SystemGuidePage = () => {
  const command = useUiCommandStore((s) => s.command);
  const clear = useUiCommandStore((s) => s.clear);

  const [systems, setSystems] = useState<any[]>([]);
  const [files, setFiles] = useState<any[]>([]);
  const [selectedSystem, setSelectedSystem] = useState<any | null>(null);

  const [loadingSystem, setLoadingSystem] = useState(false);
  const [loadingFiles, setLoadingFiles] = useState(false);

  // Load danh sách hệ thống (logic cũ)
  useEffect(() => {
    if (command !== "QUERY") return;

    setLoadingSystem(true);
    getSystems()
      .then(setSystems)
      .finally(() => {
        setLoadingSystem(false);
        clear();
      });
  }, [command, clear]);

  // Khi chọn system → load file
  const onSelectSystem = (record: any) => {
    setSelectedSystem(record);
    setFiles([]);
    setLoadingFiles(true);

    getSystemGuideFiles(record.code)
      .then(setFiles)
      .finally(() => setLoadingFiles(false));
  };

  return (
    <>
      <Title level={4}>Hướng dẫn sử dụng hệ thống</Title>

      {/* TableLayoutPanel giả lập */}
      <Row gutter={16}>
        {/* ===== LEFT – 40% ===== */}
        <Col span={9}>
          <Table
            rowKey="code"
            loading={loadingSystem}
            dataSource={systems}
            size="small"
            onRow={(record) => ({
              onClick: () => onSelectSystem(record),
            })}
            rowClassName={(record) =>
              record.code === selectedSystem?.code ? "row-selected" : ""
            }
            columns={[
              {
                title: "Tên hệ thống",
                dataIndex: "name",
              },
            ]}
          />
        </Col>

        {/* ===== RIGHT – 60% ===== */}
        <Col span={15}>
          <Table
            rowKey="id"
            loading={loadingFiles}
            dataSource={files}
            size="small"
            title={() =>
              selectedSystem
                ? `File hướng dẫn - ${selectedSystem.name}`
                : "Chọn hệ thống để xem file hướng dẫn"
            }
            columns={[
              {
                title: "Tên file",
                dataIndex: "fileName",
              },
              {
                title: "Mô tả",
                dataIndex: "description",
              },
              {
                title: "Ngày tạo",
                dataIndex: "createdAt",
              },
            ]}
          />
        </Col>
      </Row>
    </>
  );
};

export default SystemGuidePage;

