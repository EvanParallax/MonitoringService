using System.Collections.Generic;

namespace Server.DTOs
{
    public class NewDataDTO
    {
        public List<NewSensorDTO> NewSensors { get; set; }
        public List<ContainerDTO> NewContainers { get; set; }
    }
}