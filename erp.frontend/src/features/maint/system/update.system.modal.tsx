import { Modal, Form, Input, Switch, message } from "antd";
import { updateSystemModule, type SystemModuleDto } from "../../../api/system.api";
import { useEffect } from "react";

interface Props {
  open: boolean;
  data: SystemModuleDto | null;
  onClose: () => void;
  onSuccess: (updatedItem: SystemModuleDto) => void;
}

const UpdateSystemModal = ({ open, data, onClose, onSuccess }: Props) => {
  const [form] = Form.useForm();

  useEffect(() => {
    if (data) {
      form.setFieldsValue({
        name: data.name,
        isActive: data.isActive,
      });
    }
  }, [data, form]);

  const onOk = async () => {
    if (!data) return;

    const values = await form.validateFields();

    await updateSystemModule(data.code, values);

    const updatedItem: SystemModuleDto = {
      ...data,
      ...values,
      updatedAt: new Date().toISOString(),
    };

    message.success("Cập nhật hệ thống thành công");

    onSuccess(updatedItem); // ✅ FIX TYPE ERROR
    onClose();
  };

  return (
    <Modal
      open={open}
      title="Cập nhật hệ thống"
      onCancel={onClose}
      onOk={onOk}
      okText="Lưu"
      cancelText="Hủy"
      destroyOnClose
    >
      <Form form={form} layout="vertical">
        <Form.Item
          label="Tên hệ thống"
          name="name"
          rules={[{ required: true, message: "Nhập tên hệ thống" }]}
        >
          <Input />
        </Form.Item>

        <Form.Item
          label="Kích hoạt"
          name="isActive"
          valuePropName="checked"
        >
          <Switch />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default UpdateSystemModal;
