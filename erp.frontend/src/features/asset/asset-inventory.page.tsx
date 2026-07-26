import React, { useEffect, useState } from "react";
import {
  Button,
  Card,
  Divider,
  Form,
  Input,
  InputNumber,
  List,
  Modal,
  Space,
  Spin,
  Table,
  Tabs,
  Typography,
  message,
} from "antd";
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  SaveOutlined,
  SearchOutlined,
} from "@ant-design/icons";
import { useAuthStore } from "../../auth/auth.store";
import { useUiCommandStore } from "../../stores/ui-command.store";
import {
  createInventoryOrder,
  getInventoryOrder,
  inventoryCheck,
  searchInventoryOrders,
  updateInventoryOrder,
} from "../../api/inventory.api";

const { Text } = Typography;

type OrderRow = {
  id: string;
  modelId: number;
  departmentId?: string;
  inventoryType: string;
  expectedCount: number;
  counted: number;
  createdAt: string;
  createdByUserName?: string;
};

const PAGE_SIZE = 20;

const AssetInventoryPage: React.FC = () => {
  const username = useAuthStore((s) => s.username);

  // shared toolbar command
  const command = useUiCommandStore((s) => s.command);
  const clearCommand = useUiCommandStore((s) => s.clear);

  // orders list state
  const [orders, setOrders] = useState<OrderRow[]>([]);
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize] = useState(PAGE_SIZE);
  const [total, setTotal] = useState(0);
  const [loadingList, setLoadingList] = useState(false);

  const [searchModelId, setSearchModelId] = useState<number | undefined>();
  const [searchDept, setSearchDept] = useState<string | undefined>();

  // selected order detail
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [orderDetail, setOrderDetail] = useState<any | null>(null);
  const [loadingDetail, setLoadingDetail] = useState(false);

  // modal for create/update order
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingOrder, setEditingOrder] = useState<OrderRow | null>(null);
  const [form] = Form.useForm();
  const [savingOrder, setSavingOrder] = useState(false);

  // scan / count buffers
  const [scanInput, setScanInput] = useState("");
  const [scanItems, setScanItems] = useState<
    Array<{ code: string; note?: string; isDamaged?: boolean }>
  >([]);
  const [countedQty, setCountedQty] = useState<number | undefined>();
  const [performingCheck, setPerformingCheck] = useState(false);

  // initial load
  useEffect(() => {
    loadOrders();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pageIndex, searchModelId, searchDept]);

  // respond to toolbar commands
  useEffect(() => {
    if (!command) return;

    switch (command) {
      case "QUERY":
        setPageIndex(1);
        loadOrders();
        break;

      case "INSERT":
        form.resetFields();
        setEditingOrder(null);
        setIsModalOpen(true);
        break;

      // can extend MODIFY / DELETE handling here
      default:
        break;
    }

    clearCommand();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [command]);

  async function loadOrders() {
    try {
      setLoadingList(true);
      const res = await searchInventoryOrders({
        modelId: searchModelId,
        departmentId: searchDept,
        pageIndex,
        pageSize,
      });
      setOrders(res.items || []);
      setTotal(res.total || 0);
    } catch (err: any) {
      message.error(err?.message || "L?i khi t?i danh s�ch ??n ki?m");
    } finally {
      setLoadingList(false);
    }
  }

  async function loadOrderDetail(id: string) {
    try {
      setLoadingDetail(true);
      const res = await getInventoryOrder(id);
      setOrderDetail(res);
      setSelectedOrderId(id);
      // reset buffers
      setScanItems([]);
      setScanInput("");
      setCountedQty(undefined);
    } catch (err: any) {
      message.error(err?.message || "L?i khi t?i chi ti?t ?");
    } finally {
      setLoadingDetail(false);
    }
  }

  function openCreateModal() {
    setEditingOrder(null);
    form.resetFields();
    setIsModalOpen(true);
  }

  function openEditModal(order: OrderRow) {
    setEditingOrder(order);
    form.setFieldsValue({
      modelId: order.modelId,
      departmentId: order.departmentId,
      note: undefined,
    });
    setIsModalOpen(true);
  }

  async function onModalOk() {
    try {
      const vals = await form.validateFields();
      setSavingOrder(true);
      if (editingOrder) {
        await updateInventoryOrder(editingOrder.id, { departmentId: vals.departmentId, note: vals.note }, username);
        message.success("C?p nh?t th�nh c�ng");
      } else {
        await createInventoryOrder({ modelId: vals.modelId, departmentId: vals.departmentId, note: vals.note }, username);
        message.success("T?o th�nh c�ng");
      }
      setIsModalOpen(false);
      loadOrders();
    } catch (err: any) {
      message.error(err?.message || "L?i l?u ??n");
    } finally {
      setSavingOrder(false);
    }
  }

  function addScan() {
    const code = (scanInput || "").trim();
    if (!code) return;
    setScanItems((s) => {
      if (s.some((i) => i.code.toLowerCase() === code.toLowerCase())) return s;
      return [...s, { code }];
    });
    setScanInput("");
  }

  function removeScan(code: string) {
    setScanItems((s) => s.filter((i) => i.code !== code));
  }

  async function performCheck() {
    if (!selectedOrderId || !orderDetail) {
      message.error("Vui l�ng ch?n ??n ki?m tr??c");
      return;
    }

    try {
      setPerformingCheck(true);

      const body =
        orderDetail.inventoryType === "1"
          ? { inventoryId: selectedOrderId, modelId: orderDetail.modelId, departmentId: orderDetail.departmentId, items: scanItems }
          : { inventoryId: selectedOrderId, modelId: orderDetail.modelId, departmentId: orderDetail.departmentId, countedQuantity: countedQty };

      const res = await inventoryCheck(body, username);
      message.success(`K?t qu?: Found=${res.counted}, Missing=${res.missingCount ?? 0}, Unexpected=${res.unexpectedCount ?? 0}`);

      // reload detail & list
      await loadOrderDetail(selectedOrderId);
      loadOrders();
    } catch (err: any) {
      message.error(err?.message || "L?i khi th?c hi?n ki?m k�");
    } finally {
      setPerformingCheck(false);
    }
  }

  const columns = [
    { title: "ID", dataIndex: "id", key: "id" },
    { title: "ModelId", dataIndex: "modelId", key: "modelId" },
    { title: "Dept", dataIndex: "departmentId", key: "departmentId" },
    { title: "Type", dataIndex: "inventoryType", key: "inventoryType" },
    { title: "Expected", dataIndex: "expectedCount", key: "expectedCount" },
    { title: "Counted", dataIndex: "counted", key: "counted" },
    {
      title: "Action",
      key: "action",
      render: (_: any, record: any) => (
        <Space>
          <Button onClick={() => loadOrderDetail(record.id)}>Detail</Button>
          <Button onClick={() => openEditModal(record)} icon={<EditOutlined />} />
        </Space>
      ),
    },
  ];

  return (
    <Card>
      <Tabs defaultActiveKey="orders">
        <Tabs.TabPane tab="??n ki?m" key="orders">
          <div style={{ display: "flex", gap: 12, marginBottom: 12 }}>
            <InputNumber placeholder="ModelId" value={searchModelId} onChange={(v) => setSearchModelId(v as number|undefined)} />
            <Input placeholder="DepartmentId" value={searchDept} onChange={(e) => setSearchDept(e.target.value)} />
            <Button icon={<SearchOutlined />} onClick={() => { setPageIndex(1); loadOrders(); }}>Query</Button>

            <Space style={{ marginLeft: "auto" }}>
              <Button type="primary" icon={<PlusOutlined />} onClick={openCreateModal}>Insert</Button>
            </Space>
          </div>

          <Spin spinning={loadingList}>
            <Table rowKey="id" dataSource={orders} columns={columns} pagination={{ current: pageIndex, pageSize, total, onChange: (p) => setPageIndex(p) }} />
          </Spin>

          <Modal title={editingOrder ? "Modify Order" : "Insert Order"} open={isModalOpen} onOk={onModalOk} onCancel={() => setIsModalOpen(false)} confirmLoading={savingOrder}>
            <Form form={form} layout="vertical">
              <Form.Item name="modelId" label="ModelId" rules={[{ required: !editingOrder, message: "Required" }]}>
                <InputNumber disabled={!!editingOrder} style={{ width: "100%" }} />
              </Form.Item>
              <Form.Item name="departmentId" label="DepartmentId"><Input /></Form.Item>
              <Form.Item name="note" label="Note"><Input.TextArea /></Form.Item>
            </Form>
          </Modal>
        </Tabs.TabPane>

        <Tabs.TabPane tab="Chi ti?t ??n" key="detail">
          {!selectedOrderId && <Text type="secondary">Ch?n m?t ??n t?i tab ??n ki?m ?? xem chi ti?t.</Text>}

          {loadingDetail && <Spin />}

          {orderDetail && !loadingDetail && (
            <>
              <div style={{ marginBottom: 12 }}>
                <Text strong>Order:</Text> {orderDetail.id} <Divider type="vertical" />
                <Text strong>Model:</Text> {orderDetail.modelId} <Divider type="vertical" />
                <Text strong>Expected:</Text> {orderDetail.expectedCount} <Divider type="vertical" />
                <Text strong>Counted:</Text> {orderDetail.counted}
              </div>

              {orderDetail.inventoryType === "1" ? (
                <>
                  <div style={{ display: "flex", gap: 8, marginBottom: 8 }}>
                    <Input placeholder="Scan or paste code" value={scanInput} onChange={(e) => setScanInput(e.target.value)} onPressEnter={addScan} style={{ width: 420 }} />
                    <Button onClick={addScan} icon={<PlusOutlined />}>Add</Button>
                    <Button type="primary" onClick={performCheck} loading={performingCheck}>Perform Check</Button>
                  </div>

                  <List bordered dataSource={scanItems} renderItem={item => (
                    <List.Item actions={[<Button danger onClick={() => removeScan(item.code)}>Remove</Button>]}>
                      <div style={{ display: "flex", justifyContent: "space-between", width: "100%" }}>
                        <div style={{ fontWeight: 600 }}>{item.code}</div>
                        <div>{item.note}</div>
                      </div>
                    </List.Item>
                  )} />
                </>
              ) : (
                <>
                  <Form layout="inline">
                    <Form.Item label="Counted">
                      <InputNumber value={countedQty} onChange={(v) => setCountedQty(v as number|undefined)} />
                    </Form.Item>
                    <Form.Item>
                      <Button type="primary" onClick={performCheck} loading={performingCheck}>Submit Count</Button>
                    </Form.Item>
                  </Form>
                </>
              )}

              <Divider />
              <Text strong>Details</Text>
              <List bordered dataSource={orderDetail.details || []} renderItem={(d: any) => (
                <List.Item>
                  <div style={{ width: "100%", display: "flex", justifyContent: "space-between" }}>
                    <div>{d.assetNo} ({d.status})</div>
                    <div>{d.note}</div>
                  </div>
                </List.Item>
              )} />
            </>
          )}
        </Tabs.TabPane>
      </Tabs>
    </Card>
  );
};

export default AssetInventoryPage;