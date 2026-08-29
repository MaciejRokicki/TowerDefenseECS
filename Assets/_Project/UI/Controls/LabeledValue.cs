using Unity.Properties;
using UnityEngine.UIElements;

namespace TD.UI.Controls
{
    [UxmlElement]
    public partial class LabeledValue : VisualElement
    {
        private readonly Label nameLabel;
        private readonly Label valueLabel;

        private string label = "Label";
        private string value = "Value";

        [UxmlAttribute]
        [CreateProperty]
        public string Label { get => label; set => SetLabelInternal(value); }
        [UxmlAttribute]
        [CreateProperty]
        public string Value { get => value; set => SetValueInternal(value); }

        public LabeledValue()
        {
            var root = new VisualElement()
            {
                style =
                {
                    display = DisplayStyle.Flex,
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                }
            };
            nameLabel = new Label(label);
            valueLabel = new Label(value);
            root.Add(nameLabel);
            root.Add(valueLabel);
            hierarchy.Add(root);
        }

        public LabeledValue(string label, string value) : this()
        {
            SetLabelInternal(label);
            SetValueInternal(value);
        }

        private void SetLabelInternal(string value)
        {
            if (label == value)
                return;

            label = value;
            nameLabel.text = label;
        }

        private void SetValueInternal(string value)
        {
            if (this.value == value)
                return;

            this.value = value;
            valueLabel.text = value;
        }
    }
}
